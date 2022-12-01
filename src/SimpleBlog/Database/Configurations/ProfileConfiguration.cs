using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleBlog.Models;

namespace SimpleBlog.Database.Configurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public const string TableName = "SimpleBlog_Profiles";
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable(TableName);
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired();

        builder.HasOne(p => p.Account).WithOne(a => a.Profile).HasForeignKey<Profile>(p => p.AccountId).IsRequired();

    }
}