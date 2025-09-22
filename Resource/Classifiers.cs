//THIS CODE IS GENERATED!
//TO REGENERATE CALL localhost/CodeFactory/ClassifiersV1 WHILE IN DEBUG MODE AND REPLACE.

using eFlex.Index.Base.Models.App;

namespace eFlex.Index.Demo.Resource
{

	/// <summary>
	/// Generated class mirroring all classifier values.
	/// Generated from connection User ID=postgres;Password=ewqdsacxz;Host=localhost;Port=5433;Database=frl1;Include Error Detail=true;
	/// </summary>
	public static class Classifiers
	{
		public static AutoProcedure<eFlex.Index.Base.Models.App.ClassifierModel> ClassifierProcedure { get; } = AutoProcedure.Of<eFlex.Index.Base.Models.App.ClassifierModel>();
		public static AutoProcedure<eFlex.Index.Base.Models.App.ClassifierValueModel> ClassifierValueProcedure { get; } = AutoProcedure.Of<eFlex.Index.Base.Models.App.ClassifierValueModel>();


		//31d2958b-8107-47bb-a3af-f096b937c389
		public static class _Emailtemplatetype
		{
			public const string Name = "Email template type";
			public const string Code = "Emailtemplatetype";
			public const string Description = "";
			public static eFlex.Index.Base.Models.App.ClassifierModel Model => ClassifierProcedure.GetBy(nameof(eFlex.Index.Base.Models.App.ClassifierModel.Code), Code) ?? throw new ApplicationException($"Classifier '{Code}' does not exist. Update with CodeFactory.");
			public static Guid Id => Model.Id!.Value;
			public const int ValueCount = 6;

			//31d29559-06e3-4bf3-a492-4c0d3ef2caf4
			public static class _Welcome
			{
				public const string Name = "Welcome";
				public const string Code = "Welcome";
				public const int Sequence = 2;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _Emailtemplatetype.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d2954a-002a-43e7-ba37-18a10ac2cb32
			public static class _RenewToken
			{
				public const string Name = "Renew token";
				public const string Code = "RenewToken";
				public const int Sequence = 1;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _Emailtemplatetype.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d295e3-ed71-4eb2-8d8c-b0becc1bdb8b
			public static class _ForgotPassword
			{
				public const string Name = "Forgot password";
				public const string Code = "ForgotPassword";
				public const int Sequence = 3;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _Emailtemplatetype.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d2959e-86f7-449f-9c7a-69c2b14b1c87
			public static class _ProjectEstimateSend
			{
				public const string Name = "ProjectEstimateSend";
				public const string Code = "ProjectEstimateSend";
				public const int Sequence = 4;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _Emailtemplatetype.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d29553-7cca-4090-94a8-1c0a1dcc632a
			public static class _ProjectEstimateUpdate
			{
				public const string Name = "ProjectEstimateUpdate";
				public const string Code = "ProjectEstimateUpdate";
				public const int Sequence = 5;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _Emailtemplatetype.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d29545-3b60-4ae1-89b2-8f85ff0a308f
			public static class _ProjectEstimateReminder
			{
				public const string Name = "ProjectEstimateReminder";
				public const string Code = "ProjectEstimateReminder";
				public const int Sequence = 6;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _Emailtemplatetype.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}
		}

		//31d295ed-89ea-442c-bc30-d4b203763433
		public static class _InventoryType
		{
			public const string Name = "Inventory Type";
			public const string Code = "InventoryType";
			public const string Description = "";
			public static eFlex.Index.Base.Models.App.ClassifierModel Model => ClassifierProcedure.GetBy(nameof(eFlex.Index.Base.Models.App.ClassifierModel.Code), Code) ?? throw new ApplicationException($"Classifier '{Code}' does not exist. Update with CodeFactory.");
			public static Guid Id => Model.Id!.Value;
		}

		//31d29565-6746-41e8-8f1d-54eb3ffb0071
		public static class _InventoryStatusAction
		{
			public const string Name = "Inventory Status Action";
			public const string Code = "InventoryStatusAction";
			public const string Description = "";
			public static eFlex.Index.Base.Models.App.ClassifierModel Model => ClassifierProcedure.GetBy(nameof(eFlex.Index.Base.Models.App.ClassifierModel.Code), Code) ?? throw new ApplicationException($"Classifier '{Code}' does not exist. Update with CodeFactory.");
			public static Guid Id => Model.Id!.Value;
			public const int ValueCount = 5;

			//31d295d8-7863-45d9-a319-2cc0e2e39958
			public static class _Requested
			{
				public const string Name = "Requested";
				public const string Code = "Requested";
				public const int Sequence = 1;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _InventoryStatusAction.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d29534-9d33-4e76-9a5a-b2ea3c183c57
			public static class _Approved
			{
				public const string Name = "Approved";
				public const string Code = "Approved";
				public const int Sequence = 2;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _InventoryStatusAction.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d29557-7a69-4c0b-a56c-370ffe899306
			public static class _Declined
			{
				public const string Name = "Declined";
				public const string Code = "Declined";
				public const int Sequence = 3;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _InventoryStatusAction.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d29533-461e-441a-87ad-872369e6b85c
			public static class _Received
			{
				public const string Name = "Received";
				public const string Code = "Received";
				public const int Sequence = 4;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _InventoryStatusAction.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d29537-2e3b-415c-82d7-c61109d4e8d6
			public static class _Closed
			{
				public const string Name = "Closed";
				public const string Code = "Closed";
				public const int Sequence = 5;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _InventoryStatusAction.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}
		}

		//31d29599-60c2-4e35-ac3d-a390d4a4e17f
		public static class _UserDocumentType
		{
			public const string Name = "User Document Type";
			public const string Code = "UserDocumentType";
			public const string Description = "";
			public static eFlex.Index.Base.Models.App.ClassifierModel Model => ClassifierProcedure.GetBy(nameof(eFlex.Index.Base.Models.App.ClassifierModel.Code), Code) ?? throw new ApplicationException($"Classifier '{Code}' does not exist. Update with CodeFactory.");
			public static Guid Id => Model.Id!.Value;
			public const int ValueCount = 6;

			//31d29572-9a93-40f6-a607-22bf619d7107
			public static class _Image
			{
				public const string Name = "Image";
				public const string Code = "Image";
				public const int Sequence = 1;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _UserDocumentType.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d295ac-6413-4e4f-a022-e1deaee632a2
			public static class _DriversLicense
			{
				public const string Name = "Drivers License";
				public const string Code = "DriversLicense";
				public const int Sequence = 2;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _UserDocumentType.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d295b9-84f9-488f-8b49-ecb0df7f8a3d
			public static class _PersonDocument
			{
				public const string Name = "Person Document";
				public const string Code = "PersonDocument";
				public const int Sequence = 3;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _UserDocumentType.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d295b8-a12b-4f6a-b1ca-3a1361a234df
			public static class _EducationDocument
			{
				public const string Name = "Education Document";
				public const string Code = "EducationDocument";
				public const int Sequence = 4;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _UserDocumentType.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d2957b-822f-408f-bcc4-64fc8b51291d
			public static class _Qualification
			{
				public const string Name = "Qualification";
				public const string Code = "Qualification";
				public const int Sequence = 5;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _UserDocumentType.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d29543-7aff-45ca-97e9-bacdd1db1c9a
			public static class _Other
			{
				public const string Name = "Other";
				public const string Code = "Other";
				public const int Sequence = 6;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _UserDocumentType.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}
		}

		//31d295db-d1ac-4195-8461-a02143a2b82d
		public static class _UserDocumentStatus
		{
			public const string Name = "User Document Status";
			public const string Code = "UserDocumentStatus";
			public const string Description = "";
			public static eFlex.Index.Base.Models.App.ClassifierModel Model => ClassifierProcedure.GetBy(nameof(eFlex.Index.Base.Models.App.ClassifierModel.Code), Code) ?? throw new ApplicationException($"Classifier '{Code}' does not exist. Update with CodeFactory.");
			public static Guid Id => Model.Id!.Value;
			public const int ValueCount = 4;

			//31d295a2-b8af-4fc9-931b-a5bf14363b82
			public static class _Pending
			{
				public const string Name = "Pending";
				public const string Code = "Pending";
				public const int Sequence = 1;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _UserDocumentStatus.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d29575-8232-4daa-af84-da41de583243
			public static class _Approved
			{
				public const string Name = "Approved";
				public const string Code = "Approved";
				public const int Sequence = 2;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _UserDocumentStatus.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d295fc-8c15-47e3-a04e-5e855f23b1e1
			public static class _Declined
			{
				public const string Name = "Declined";
				public const string Code = "Declined";
				public const int Sequence = 3;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _UserDocumentStatus.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d2958b-b395-44d7-b67b-b53f420a4571
			public static class _Expired
			{
				public const string Name = "Expired";
				public const string Code = "Expired";
				public const int Sequence = 4;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _UserDocumentStatus.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}
		}

		//31d2957f-76f5-4a0c-9862-55a9649c1cb1
		public static class _DriversLicense
		{
			public const string Name = "Drivers License";
			public const string Code = "DriversLicense";
			public const string Description = "";
			public static eFlex.Index.Base.Models.App.ClassifierModel Model => ClassifierProcedure.GetBy(nameof(eFlex.Index.Base.Models.App.ClassifierModel.Code), Code) ?? throw new ApplicationException($"Classifier '{Code}' does not exist. Update with CodeFactory.");
			public static Guid Id => Model.Id!.Value;
		}

		//31d29523-e615-43b3-8afa-8e4eb047e1b0
		public static class _Education
		{
			public const string Name = "Education";
			public const string Code = "Education";
			public const string Description = "";
			public static eFlex.Index.Base.Models.App.ClassifierModel Model => ClassifierProcedure.GetBy(nameof(eFlex.Index.Base.Models.App.ClassifierModel.Code), Code) ?? throw new ApplicationException($"Classifier '{Code}' does not exist. Update with CodeFactory.");
			public static Guid Id => Model.Id!.Value;
		}

		//31d29593-d5a7-4c6b-945c-19fdfd2de446
		public static class _PersonDocument
		{
			public const string Name = "Person Document";
			public const string Code = "PersonDocument";
			public const string Description = "";
			public static eFlex.Index.Base.Models.App.ClassifierModel Model => ClassifierProcedure.GetBy(nameof(eFlex.Index.Base.Models.App.ClassifierModel.Code), Code) ?? throw new ApplicationException($"Classifier '{Code}' does not exist. Update with CodeFactory.");
			public static Guid Id => Model.Id!.Value;
		}

		//31d29578-5dff-4e24-b22f-3bb4867259a3
		public static class _LanguageSkillsLanguage
		{
			public const string Name = "Language Skills Language";
			public const string Code = "LanguageSkillsLanguage";
			public const string Description = "";
			public static eFlex.Index.Base.Models.App.ClassifierModel Model => ClassifierProcedure.GetBy(nameof(eFlex.Index.Base.Models.App.ClassifierModel.Code), Code) ?? throw new ApplicationException($"Classifier '{Code}' does not exist. Update with CodeFactory.");
			public static Guid Id => Model.Id!.Value;
		}

		//31d29504-f27a-43ee-b99f-87f189faf03b
		public static class _LanguageSkillsLevel
		{
			public const string Name = "Language Skills Level";
			public const string Code = "LanguageSkillsLevel";
			public const string Description = "";
			public static eFlex.Index.Base.Models.App.ClassifierModel Model => ClassifierProcedure.GetBy(nameof(eFlex.Index.Base.Models.App.ClassifierModel.Code), Code) ?? throw new ApplicationException($"Classifier '{Code}' does not exist. Update with CodeFactory.");
			public static Guid Id => Model.Id!.Value;
		}

		//31d295c7-8439-481a-8409-f6fe7615566e
		public static class _Qualification
		{
			public const string Name = "Qualification";
			public const string Code = "Qualification";
			public const string Description = "";
			public static eFlex.Index.Base.Models.App.ClassifierModel Model => ClassifierProcedure.GetBy(nameof(eFlex.Index.Base.Models.App.ClassifierModel.Code), Code) ?? throw new ApplicationException($"Classifier '{Code}' does not exist. Update with CodeFactory.");
			public static Guid Id => Model.Id!.Value;
		}

		//31d2952a-027a-4d4f-9653-24e7c62d0ea0
		public static class _ProjectStatus
		{
			public const string Name = "Project Status";
			public const string Code = "ProjectStatus";
			public const string Description = "";
			public static eFlex.Index.Base.Models.App.ClassifierModel Model => ClassifierProcedure.GetBy(nameof(eFlex.Index.Base.Models.App.ClassifierModel.Code), Code) ?? throw new ApplicationException($"Classifier '{Code}' does not exist. Update with CodeFactory.");
			public static Guid Id => Model.Id!.Value;
			public const int ValueCount = 7;

			//31d295ad-944e-44b8-9736-ee475c7c4c9f
			public static class _New
			{
				public const string Name = "New";
				public const string Code = "New";
				public const int Sequence = 1;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _ProjectStatus.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d2956e-fa10-4f26-983b-111f1f245dde
			public static class _Waiting
			{
				public const string Name = "Waiting";
				public const string Code = "Waiting";
				public const int Sequence = 2;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _ProjectStatus.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d29529-1185-4b89-9f3f-8bf7fe9e6e54
			public static class _Approved
			{
				public const string Name = "Approved";
				public const string Code = "Approved";
				public const int Sequence = 3;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _ProjectStatus.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d2955a-50fa-420a-ade8-64b7bd977698
			public static class _Rejected
			{
				public const string Name = "Rejected";
				public const string Code = "Rejected";
				public const int Sequence = 4;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _ProjectStatus.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d29500-5971-4997-95b2-ac64bfeaae81
			public static class _Active
			{
				public const string Name = "Active";
				public const string Code = "Active";
				public const int Sequence = 5;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _ProjectStatus.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d29502-7d08-443c-86d2-91697e5581b7
			public static class _Completed
			{
				public const string Name = "Completed";
				public const string Code = "Completed";
				public const int Sequence = 6;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _ProjectStatus.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d29503-7c2e-4d58-9a68-ba459f2726b2
			public static class _Caanceled
			{
				public const string Name = "Caanceled";
				public const string Code = "Caanceled";
				public const int Sequence = 7;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _ProjectStatus.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}
		}

		//31d295ea-7aab-44f0-a731-4b8f627ae7bc
		public static class _ProjectEstimateStatus
		{
			public const string Name = "Project Estimate Status";
			public const string Code = "ProjectEstimateStatus";
			public const string Description = "";
			public static eFlex.Index.Base.Models.App.ClassifierModel Model => ClassifierProcedure.GetBy(nameof(eFlex.Index.Base.Models.App.ClassifierModel.Code), Code) ?? throw new ApplicationException($"Classifier '{Code}' does not exist. Update with CodeFactory.");
			public static Guid Id => Model.Id!.Value;
			public const int ValueCount = 4;

			//31d29583-ad4e-4872-9207-c2b859481170
			public static class _New
			{
				public const string Name = "New";
				public const string Code = "New";
				public const int Sequence = 1;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _ProjectEstimateStatus.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d29525-b982-4cd9-9052-f921c0d1f2ab
			public static class _Waiting
			{
				public const string Name = "Waiting";
				public const string Code = "Waiting";
				public const int Sequence = 2;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _ProjectEstimateStatus.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d295b0-5ee1-4e77-90e4-43f382910000
			public static class _Approved
			{
				public const string Name = "Approved";
				public const string Code = "Approved";
				public const int Sequence = 3;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _ProjectEstimateStatus.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}

			//31d295ac-ae45-47f5-9947-035209c1a9aa
			public static class _Rejected
			{
				public const string Name = "Rejected";
				public const string Code = "Rejected";
				public const int Sequence = 4;
				public const string Notes = "";
				private static SqlWhereCondition where = new(new List<SqlWherePart>() { new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.Code), Code, SqlWherePart.CommonPatterns.StringEqual), new(nameof(eFlex.Index.Base.Models.App.ClassifierValueModel.ClassifierId), _ProjectEstimateStatus.Id) });
				public static eFlex.Index.Base.Models.App.ClassifierValueModel Model => ClassifierValueProcedure.GetRange(where).FirstOrDefault() ?? throw new ApplicationException($"Classifier value '{Code}' does not exist. Update with CodeFactory.");
				public static Guid Id => Model.Id!.Value;
			}
		}
	}
}