using eFlex.Common.Extensions;
using eFlex.Index.Base.Types;
using System.Reflection;
using System.Text.RegularExpressions;

namespace frlnet.Helpers
{
	public static class TemplateParser
	{
		public static string ReadTemplate(string fileName)
		{
			var assemblyFile = new FileInfo(Assembly.GetExecutingAssembly().Location);
			var templatePath = Path.Combine(assemblyFile.Directory!.FullName, "Resource", "Templates", fileName);
			if (!System.IO.File.Exists(templatePath))
				throw new FileNotFoundException("Template file not found.", templatePath);

			var template = System.IO.File.ReadAllText(templatePath);
			return template;
		}


		public static IEnumerable<string> ReadTemplateKeys(string template)
		{
			if (string.IsNullOrWhiteSpace(template))
				return Enumerable.Empty<string>();

			// Matches any text inside {...} but ensures there is valid content (no leading/trailing spaces, etc.)
			var regex = new Regex(@"\{(\??[a-zA-Z0-9_.]+)\}");

			var matches = regex.Matches(template);

			// Extract each captured group (the text inside { })
			return matches
				.Cast<System.Text.RegularExpressions.Match>()
				.Select(m => m.Groups[1].Value.Trim())
				.Where(key => !string.IsNullOrWhiteSpace(key)) // Ensure valid keys
				.Distinct();
		}


		public static string FillValues(string template, hIndexModel indexModel, string prefix)
		{
			// 1) Ensure that prefix ends with a dot if not empty
			if (!string.IsNullOrEmpty(prefix) && !prefix.EndsWith("."))
			{
				prefix += ".";
			}

			// 2) Gather all placeholders from the template
			var placeholders = ReadTemplateKeys(template);

			// 3) Get your control structure
			var structure = indexModel.Instructions.Edit.GetControlStructure(indexModel);
			indexModel.Instructions.Edit.ClearStructure();

			// Local function to replace both {placeholder} and {?placeholder} with a given value
			void ReplacePlaceholder(ref string input, string ph, string value)
			{
				// Replace {placeholder}
				input = input.Replace($"{{{ph}}}", value);
				// Replace {?placeholder}
				input = input.Replace($"{{?{ph}}}", value);
			}

			foreach (var placeholder in placeholders)
			{
				// 4) Skip if placeholder doesn't start with prefix or "?prefix"
				if (!placeholder.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) &&
					!placeholder.StartsWith("?" + prefix, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}

				// 5) Extract the keyPart = everything after the prefix (or "?prefix")
				//    We'll remove only the first occurrence of prefix or "?prefix".
				//    That means if placeholder = "Project.Name.Label", prefix = "Project.",
				//    then keyPart = "Name.Label".
				// 
				//    This trick uses substring length but ensure we handle the optional "?".
				var hasQuestion = placeholder.StartsWith("?");
				var trimmedPlaceholder = hasQuestion ? placeholder.Substring(1) : placeholder;

				// remove the prefix portion (e.g., "Project.") from the trimmed placeholder
				var keyPart = trimmedPlaceholder.Substring(prefix.Length);  // safe because we checked startsWith above

				// 5A) If it's a label placeholder (ends with .Label)
				if (keyPart.EndsWith(".Label", StringComparison.OrdinalIgnoreCase))
				{
					// We can fill labels even if indexModel.Model == null
					var mainKey = keyPart.Substring(0, keyPart.Length - ".Label".Length);

					var fCtrl = structure
						.GetAllControls()
						.FirstOrDefault(c => c.Key.Equals(mainKey, StringComparison.OrdinalIgnoreCase));

					if (fCtrl == null)
						continue;

					var labelValue = fCtrl.Label?.Text ?? string.Empty;
					ReplacePlaceholder(ref template, placeholder, labelValue);
				}
				else
				{
					// 5B) If Model is null => skip filling these placeholders now
					//     (We only want labels filled if Model is null.)
					if (indexModel.Model == null)
						continue;

					// 5C) Check if there's a numeric format pattern in "keyPart",
					//     e.g. "Start.f2" => mainKey = "Start", format = "f2"
					var subParts = keyPart.Split('.');
					if (subParts.Length > 1)
					{
						var lastPart = subParts.Last();
						var mainKey = string.Join(".", subParts.Take(subParts.Length - 1));

						var fCtrl = structure
							.GetAllControls()
							.FirstOrDefault(c => c.Key.Equals(mainKey, StringComparison.OrdinalIgnoreCase));

						if (fCtrl == null)
							continue;

						// Apply dropList transformation
						var finalValue = TransformDropListValue(fCtrl, indexModel);

						// Attempt numeric formatting
						if (decimal.TryParse(finalValue, out var numericValue))
						{
							try
							{
								finalValue = numericValue.ToString(lastPart);
							}
							catch
							{
								// If it's not a valid numeric format, keep the raw value
							}
						}

						ReplacePlaceholder(ref template, placeholder, finalValue);
					}
					else
					{
						// 5D) Basic case: no numeric format
						var mainKey = keyPart;

						var fCtrl = structure
							.GetAllControls()
							.FirstOrDefault(c => c.Key.Equals(mainKey, StringComparison.OrdinalIgnoreCase));

						if (fCtrl == null)
							continue;

						var finalValue = TransformDropListValue(fCtrl, indexModel);
						ReplacePlaceholder(ref template, placeholder, finalValue);
					}
				}
			}

			return template;
		}



		/// <summary>
		/// Centralizes DropListSettings logic so we don't repeat code.
		/// </summary>
		private static string TransformDropListValue(ControlModel fCtrl, hIndexModel indexModel)
		{
			// If there's no model or no value, return empty
			if (indexModel.Model == null || fCtrl.Value == null)
				return fCtrl.Value?.ToString() ?? string.Empty;

			// Convert to string
			var rawValue = fCtrl.Value.ToString() ?? string.Empty;

			// If DropListSettings is present, convert to display text
			if (fCtrl.Settings is DropListSettings dropListSettings)
			{
				var itemSource = dropListSettings.GetSelectList(
					fCtrl.Key,
					indexModel.ModelType,
					indexModel.ControllerType,
					indexModel.Model
				);
				var valueIdx = Array.IndexOf(itemSource.GetValues(), rawValue);
				if (valueIdx >= 0)
					rawValue = itemSource.GetTexts()[valueIdx];
			}

			return rawValue;
		}

		public static string ExtractTemplatePart(string template, string startTag, string endTag)
		{
			var startIndex = template.IndexOf(startTag) + startTag.Length;
			var endIndex = template.IndexOf(endTag);
			if (startIndex < 0 || endIndex < 0 || endIndex <= startIndex)
				return string.Empty;
			return template.Substring(startIndex, endIndex - startIndex).Trim();
		}

	}



}
