using System;
using System.Collections.Generic;
using System.Text;
using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFStorage.Configuration
{
    public class DetailsConfiguration:IEntityTypeConfiguration<Details>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Details> builder)
        {
            builder.HasKey(nameof(Details.FullDetails));
        }
    }
}
