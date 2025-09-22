using eFlex.Index.Base;
using eFlex.Index.Base.Models.Admin;
using eFlex.Index.Base.Models.Admin.Entity;
using eFlex.Index.Base.Models.App;
using eFlex.Index.Base.Threads;
using eFlex.Index.Base.Types;
using System.Diagnostics;

if (Debugger.IsAttached)
	Thread.CurrentThread.Name = "Startup thread";

Disabled.Registry.Add(typeof(eFlex.Index.Base.Controllers.Admin.UserController));
Disabled.Registry.Add(typeof(eFlex.Index.Base.Controllers.Admin.UserRegisterController));
Disabled.Registry.Add(typeof(eFlex.Index.Base.Controllers.Admin.ProfileController));

ThemeModel.Current.KendoTheme = Path.Combine("lib", "kendo-theme-material", "dist", "material-smoke.css");

ScriptRunner.RunAllScripts = false;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var config = builder.Configuration;

//PostgreSQL Connection must be first.
{//Initialize postgress connectivity.
	const string connectionSettingKey = "IndexDemo1";
	var connectionString = config.GetConnectionString(connectionSettingKey) ?? throw new Exception("Postgres connection string not found in AppSettings.");
	var connectivity = new Postgres(connectionSettingKey, connectionString);
	eFlex.Connectivity.Initialize.Connection(connectivity);
}


//Configure assign instances.
var indexConfig = new IndexConfigurator();
indexConfig.AddUserRegistration(null);
indexConfig.AddForgotPassword();
indexConfig.EmailAsUsername = true;

var app = builder.BuildIndex(indexConfig, Populate);
var service = builder.Services;

IndexInstructions.DefaultInstructions = CreateInstructions;

LabelTranslator.Start(new LabelTranslator.GoogleTranslate());

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static void Populate()
{
	eFlex.Index.Base.Initialize.Populate();

	var isFirstInsert = false;
	{ //Populate LabelLanguage Latvian.
		var procedure = AutoProcedure.Of<LabelLanguageModel>();
		isFirstInsert = procedure.Populate(new LabelLanguageModel() { Name = "Latvian", Key = Constants.LatvianLanguageKey, Enabled = true, Culture = "lv-LV" });
	}

	if (isFirstInsert)
	{ //Update email parameters.
		var procedure = (ParameterProcedure)AutoProcedure.Of<ParameterModel>()!;

		var model = procedure.GetByCode(Constants.ParameterCodes.MessageEmail)!;
		model.Value = "mail@frlnet.com";
		procedure.Update(model);

		model = procedure.GetByCode(Constants.ParameterCodes.MessageEmaiPassword)!;
		model.Value = "NoliktavuIela2";
		procedure.Update(model);

		model = procedure.GetByCode(Constants.ParameterCodes.MessageSMPTAddress)!;
		model.Value = "smtp.zone.eu";
		procedure.Update(model);

		model = procedure.GetByCode(Constants.ParameterCodes.MessageSMPTPort)!;
		model.Value = "587";
		procedure.Update(model);
	}

	{ //Populate classifiers.
		var ClProcedure = AutoProcedure.Of<ClassifierModel>();
		var ClvProcedure = AutoProcedure.Of<ClassifierValueModel>();

		{//Inventory Type
			var squence = 0;
			if (ClProcedure.Populate(new ClassifierModel() { Name = "Inventory Type", Dynamic = true }, out var id))
			{
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Clothing", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Foot Wear", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Lighting", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Stage Equipment", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Instruments", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Other", Sequence = ++squence });
			}
		}

		{//InventoryStatus Action
			var squence = 0;
			ClProcedure.Populate(new ClassifierModel() { Name = "Inventory Status Action", Dynamic = false }, out var id);
			ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Requested", Sequence = ++squence });
			ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Approved", Sequence = ++squence });
			ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Declined", Sequence = ++squence });
			ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Received", Sequence = ++squence });
			ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Closed", Sequence = ++squence });
		}

		Guid documentDrivers;
		Guid documentEducation;
		Guid documentQualification;
		Guid documentPersonal;
		{//User Document Type
			var squence = 0;
			ClProcedure.Populate(new ClassifierModel() { Name = "User Document Type", Dynamic = false }, out var id);
			ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Image", Sequence = ++squence });
			ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Drivers License", Sequence = ++squence }, out documentDrivers);
			ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Person Document", Sequence = ++squence }, out documentPersonal);
			ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Education Document", Sequence = ++squence }, out documentEducation);
			ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Qualification", Sequence = ++squence }, out documentQualification);
			ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Other", Sequence = ++squence });
		}

		{//User Document Status
			var squence = 0;
			ClProcedure.Populate(new ClassifierModel() { Name = "User Document Status", Dynamic = false }, out var id);
			ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Pending", Sequence = ++squence });
			ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Approved", Sequence = ++squence });
			ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Declined", Sequence = ++squence });
			ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Expired", Sequence = ++squence });
		}

		{//User Document Type and Document Additional Type

			{
				var squence = 0;
				if (ClProcedure.Populate(new ClassifierModel() { Name = "Drivers License", Dynamic = true }, out var id))
				{
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Drivers License AM", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Drivers License A1", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Drivers License A2", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Drivers License A", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Drivers License B1", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Drivers License B", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Drivers License C1", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Drivers License C", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Drivers License D1", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Drivers License D", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Drivers License BE", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Drivers License C1E", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Drivers License CE", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Drivers License D1E", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Drivers License DE", Sequence = ++squence });
				}
			}

			{
				var squence = 0;
				if (ClProcedure.Populate(new ClassifierModel() { Name = "Education", Dynamic = true }, out var id))
				{
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Certificate of general basic education", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Certificate of vocational basic education", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Certificate of general secondary education", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Certificate of vocational education", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Diploma of first level professional higher education", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Bachelors diploma", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Professional Bachelors diploma", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Masters diploma", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Professional Masters diploma", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Diploma of professional higher education", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Doctors diploma", Sequence = ++squence });
				}
			}


			{
				var squence = 0;
				if (ClProcedure.Populate(new ClassifierModel() { Name = "Person Document", Dynamic = true }, out var id))
				{
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Passport LV", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "ID Card", Sequence = ++squence });
				}
			}

			{
				var squence = 0;
				if (ClProcedure.Populate(new ClassifierModel() { Name = "Qualification", Dynamic = true }, out var id))
				{
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Forklift Operator Certification", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Cherry Picker Training", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Scissor Lift Certification", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Reach Truck Operator License", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Pallet Jack Training", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Telehandler Certification", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Dock Loader Training", Sequence = ++squence });
					ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Rigging and Hoisting Certification", Sequence = ++squence });
				}
			}

		}

		{//Project status.
			var squence = 0;
			if (ClProcedure.Populate(new ClassifierModel() { Name = "Project Status", Dynamic = false }, out var id))
			{
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "New", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Waiting", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Approved", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Rejected", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Active", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Completed", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Caanceled", Sequence = ++squence });
			}
		}

		{//Project estimate status.
			var squence = 0;
			if (ClProcedure.Populate(new ClassifierModel() { Name = "Project Estimate Status", Dynamic = false }, out var id))
			{
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "New", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Waiting", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Approved", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Rejected", Sequence = ++squence });
			}
		}

		{
			var procedure = AutoProcedure.Of<ParameterModel>();
			procedure.Populate(new ParameterModel() { Code = "UserDocumentMaxFileLengthInMB", Name = "User document maximum file length in MB", Value = 30u.ToString(), Type = typeof(uint).FullName!, Category = "UserDocument" });
		}

		{//Language skills - Language.
			var squence = 0;
			if (ClProcedure.Populate(new ClassifierModel() { Name = "Language Skills Language", Dynamic = true }, out var id))
			{
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "English", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Latviešu", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Русский", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Deutsch", Sequence = ++squence });
			}
		}

		{//Language skills - Level.
			var squence = 0;
			if (ClProcedure.Populate(new ClassifierModel() { Name = "Language Skills Level", Dynamic = true }, out var id))
			{
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Elementary proficiency", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Limited working proficiency", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Professional working proficiency", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Full professional proficiency", Sequence = ++squence });
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = id, Name = "Naticve or bilingual proficiency", Sequence = ++squence });
			}
		}
	}

	{ //Populate user registration related.

		Guid publicOrgId;
		{ //Populate Organization.
			var procedure = AutoProcedure.Of<OrganizationModel>();
			procedure.Populate(new OrganizationModel() { Name = Constants.PublicOrganizationName, RegNumber = "00000000001", Email = "admin@eFlex.test", IsMaster = true }, out publicOrgId);
			eFlex.Index.Base.Initialize.IndexConfiguration.UserRegistrationOrganizationId = publicOrgId;
		}

		Guid publicRoleId;
		{ //Populate Role.
			var procedure = AutoProcedure.Of<RoleEntityModel>();
			procedure.Populate(new RoleEntityModel() { Name = "Public", OrganizationId = publicOrgId }, out publicRoleId);
			eFlex.Index.Base.Initialize.IndexConfiguration.UserRegistrationRoleIds.Add(publicRoleId);
		}

		{ //Populate RoleClaim.
			var procedure = AutoProcedure.Of<RoleClaimEntityModel>();
			//This is for future populate of public role.
			//var claimIds = AutoProcedure.Of<ClaimEntityModel>().GetRange(useCache: false).Where(f => f.Type.ToString() == typeof(InventoryController).ToString()).Select(f => f.Id!.Value);
			//foreach (var f in claimIds)
			//    procedure.Populate(new RoleClaimEntityModel() { ClaimId = f, RoleId = publicRoleId });
		}
	}

	{//Populate settings
	 // paytraq
		var procedure = AutoProcedure.Of<SettingModel>();
		procedure.Populate(new() { Code = "PayTraqApiKey", Name = "PayTraq API Key", Type = typeof(string).FullName!, Category = "Integration", Description = "Sync client data with PayTraq API." }, out _);
	}

	IConnectivity.Default().Instances.Commit();

	{ //Populate templates.
		var ClProcedure = AutoProcedure.Of<ClassifierModel>();
		var ClvProcedure = AutoProcedure.Of<ClassifierValueModel>();
		var emailTempProcedure = AutoProcedure.Of<EmailTemplateModel>();

		{//Email templates.
			var clId = Classifiers._Emailtemplatetype.Id;
			var squence = Classifiers._Emailtemplatetype.Model.Values!.OrderByDescending(f => f.Sequence).First().Sequence;

			{
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = clId, Name = "ProjectEstimateSend", Sequence = ++squence }, out var clvId);
				emailTempProcedure.Populate(new EmailTemplateModel() { LabelLanguageId = LabelLanguageModel.DefaultId, TypeClvId = clvId, Subject = "Project estimate" });
			}

			{
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = clId, Name = "ProjectEstimateUpdate", Sequence = ++squence }, out var clvId);
				emailTempProcedure.Populate(new EmailTemplateModel() { LabelLanguageId = LabelLanguageModel.DefaultId, TypeClvId = clvId, Subject = "Project estimate status changed" });
			}

			{
				ClvProcedure.Populate(new ClassifierValueModel() { ClassifierId = clId, Name = "ProjectEstimateReminder", Sequence = ++squence }, out var clvId);
				emailTempProcedure.Populate(new EmailTemplateModel() { LabelLanguageId = LabelLanguageModel.DefaultId, TypeClvId = clvId, Subject = "Project estimate awaits confirmation" });
			}
		}
	}

	IConnectivity.Default().Instances.Commit();

}


static IndexInstructions CreateInstructions()
{
	var instructions = new IndexInstructions();
	instructions.Index.Filter.Expanded = false;
	instructions.Context.DefaultControlSize = ControlBuilder.eControlSize.c4;
	instructions.Context.DefaultLabelSize = ControlBuilder.eLabelSize.c2;
	instructions.Index.Grid.MultiselectMode = GridBuilder.eMultiselectMode.CtrlShift;
	return instructions;
}
