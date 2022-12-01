using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleBlog.Models;

namespace SimpleBlog.Database.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public const string TableName = "SimpleBlog_Posts";
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable(TableName);
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title).IsRequired();
        builder.Property(p => p.Content).IsRequired();
        builder.Property(p => p.CreatedOn).IsRequired();

        builder.HasOne(p => p.Owner).WithMany(o => o.Posts).HasForeignKey(p => p.OwnerId).IsRequired();
    }
}