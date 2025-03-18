using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.LMSIsisMapping
{
    public abstract class LMSIsisEntityTypeConfiguration<T> : IEntityTypeConfiguration<T> where T : class
    {
        protected LMSIsisEntityTypeConfiguration()
        {
            PostInitialize();
        }

        public void Configure(EntityTypeBuilder<T> builder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Developers can override this method in custom partial classes
        ///     in order to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {
        }
    }
}
