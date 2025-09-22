using Dapper;
using eFlex.Index.Base.Jobs;
using eFlex.Index.Base.Jobs.InternalModels;
using frlnet.Models.frl;
using System.Reflection;

namespace frlnet
{
    public class UserDocumentExpiredJob : JobBase
    {
        public long SetStatusBeforeExpiredInDays { get; set; } = 0;

        public override WorkStatusReporter Work()
        {
            //-1 not needed, due DbDateTimeEqualOrSmaller pattern.
            var expiredDate = DateTimeOffset.Now.Date.AddDays(SetStatusBeforeExpiredInDays);

            var where = new SqlWhereCondition();
            where.Parts.Add(new(nameof(UserDocumentExpiredJobModel.ExpirationDate), expiredDate, SqlWherePart.CommonPatterns.DbDateTimeEqualOrSmaller));
            where.Parts.Add(new(nameof(UserDocumentExpiredJobModel.StatusClvId), Classifiers._UserDocumentStatus._Expired.Id, SqlWherePart.CommonPatterns.GuidNotEqual));

            var expiredCount = AutoProcedure.Of<UserDocumentExpiredJobModel>().GetCount(where);

            var reporter = new WorkStatusReporter(0, 0, 0)
            {
                ItemsFound = (uint)expiredCount
            };
            
            if (reporter.ItemsFound == 0)
                return reporter;

            var mapSource = typeof(UserDocumentExpiredJobModel).GetCustomAttribute<MapSource>() ?? throw new Exception($"Model: {nameof(UserDocumentExpiredJobModel)} must contain {nameof(MapSource)}");

            var defaultConnection = IConnectivity.Default();

            const string paramNumber = "1";
            var parameters = new DynamicParameters();
            parameters.Add(defaultConnection.ParameterPrefix + nameof(UserDocumentExpiredJobModel.StatusClvId), Classifiers._UserDocumentStatus._Expired.Id, System.Data.DbType.Guid);
            parameters.Add(defaultConnection.ParameterPrefix + nameof(UserDocumentExpiredJobModel.StatusClvId) + paramNumber, Classifiers._UserDocumentStatus._Expired.Id, System.Data.DbType.Guid);
            parameters.Add(defaultConnection.ParameterPrefix + nameof(UserDocumentExpiredJobModel.ExpirationDate) + paramNumber, expiredDate, System.Data.DbType.DateTime);

            var sql = $"UPDATE {mapSource.Schema}.{defaultConnection.FormatName(mapSource.Source)} " + Environment.NewLine;
            sql += $"SET {defaultConnection.FormatName(nameof(UserDocumentExpiredJobModel.StatusClvId))} = {defaultConnection.ParameterPrefix}{nameof(UserDocumentExpiredJobModel.StatusClvId)} " + Environment.NewLine;
            sql += $", {defaultConnection.FormatName(nameof(UserDocumentExpiredJobModel.RowModified))} = CURRENT_TIMESTAMP(3)" + Environment.NewLine;
            sql += $"{where.GetQuery(defaultConnection)}";

            try
            {
                Sql.Query<Guid>(defaultConnection, sql, parameters, requireTransaction: true);
                reporter.ItemsCompleted = reporter.ItemsFound;
                IConnectivity.Default().Instances.Commit();
            }
            catch (Exception ex)
            {
                reporter.ItemsErrored = reporter.ItemsFound;
                Log(JobDetailsModel.eLevel.Error, ex.Message);
            }

            return reporter;
        }
    }
}
