using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace SIDS.Plugin.Payments.BetterStripe.Mapping.Builders
{
    public class PluginBuilder : NopEntityBuilder<SIDS.Plugin.Payments..BetterStripe.Domains.CustomTable>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
        }

        #endregion
    }
}