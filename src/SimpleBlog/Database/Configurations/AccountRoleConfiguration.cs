using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleBlog.Models;

namespace SimpleBlog.Database.Configurations;

public class AccountRoleConfiguration : IEntityTypeConfiguration<AccountRole>
{
    public void Configure(EntityTypeBuilder<AccountRole> builder)
    {
        builder.HasMany(r => r.Claims).WithOne(c => c.Role).HasForeignKey(c => c.RoleId).IsRequired(false);
    }
}