using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using SIDS.Plugin.Misc.SEOCleaner.Domains;

namespace SIDS.Plugin.Misc.SEOCleaner.Mapping.Builders
{
    public class PluginBuilder : NopEntityBuilder<CustomTable>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
        }

        #endregion
    }
}