using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleBlog.Models;

namespace SimpleBlog.Database.Configurations;

public class PostTagConfiguration : IEntityTypeConfiguration<PostTag>
{
    public const string TableName = "SimpleBlog_PostTags";
    public void Configure(EntityTypeBuilder<PostTag> builder)
    {
        builder.ToTable(TableName);
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title).HasMaxLength(20).IsRequired();
    }
}
