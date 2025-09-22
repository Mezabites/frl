using eFlex.Index.Base.ActionResults;
using frlnet.Jobs;
using frlnet.Models.frl;
namespace frlnet.Controllers.frl
{
	[ClaimFrom<ClientController>]
	public class ClientContactController : hAreaIndexController<ClientContactModel>
	{
		public ClientContactController()
		{
			Instructions.Index.Grid.Buttons.Delete.Visible = false;
			Instructions.Create.AfterSave = CreateBuilder.eAfterSave.ReturnToPrevius;
			Instructions.Index.Filter.SearchParentReferences = true;
		}

		//protected override SqlWhereCondition TranslateFilter(hIndexModel indexModel)
		//{
		//	var clientId = IndexNavigation.Current.GetQueryGuid<ClientController>();
		//	if(clientId.HasValue)
		//		var clientModel = 
		//	return base.TranslateFilter(indexModel);
		//}

		protected override ClientContactModel CreateViewModel()
		{
			var model = base.CreateViewModel();
			if (model.ParentId == Guid.Empty)
			{
				var indexModel = IndexRegistyFilter.GetModel(PageId!.Value, typeof(ProjectController)) as IndexModel<ProjectModel>;
				if(indexModel is not null)
				{
					model.ParentId = indexModel.ExactModel.ClientId;
				}
				else
				{
					var projectId = IndexNavigation.Current.GetQueryGuid<ProjectController>();
					if(projectId.HasValue)
					{
						var projectModel = AutoProcedure.Of<ProjectModel>().Get(projectId.Value)!;
						model.ParentId = projectModel.ClientId;
					}	
				}
			}
			return model;
		}

		public override IndexResult CreateConfirm(ClientContactModel model)
		{
			var res = base.CreateConfirm(model);
			if (!res.HasErrors)
			{
				var clientModel = AutoProcedure.Of<ClientModel>().Get(model.ParentId);
				var apiCom = ApiSync.GetApiCom(clientModel!.OrganizationId);
				if (apiCom != null)
				{
					apiCom.InsertSingle<ClientContactModel>(model, clientModel);
					Procedure.Connectivity.Instances.Commit();
					ApiSync.Update([clientModel], null, apiCom.SyncModel);
				}
			}
			return res;
		}

		public override IndexResult EditConfirm(ClientContactModel model)
		{
			var res = base.EditConfirm(model);
			if (!res.HasErrors)
			{
				var apiCom = ApiSync.GetApiCom(model.OrganizationId);
				if (apiCom != null)
				{
					var clientModel = AutoProcedure.Of<ClientModel>().Get(model.ParentId);
					apiCom?.UpdateSingle<ClientContactModel>(model, clientModel);
					Procedure.Connectivity.Instances.Commit();
					ApiSync.SyncLinked([model], clientModel);
				}
			}
			return res;
		}

	}
}
