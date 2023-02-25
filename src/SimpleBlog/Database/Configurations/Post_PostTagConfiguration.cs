using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleBlog.Models;

namespace SimpleBlog.Database.Configurations;

public class Post_PostTagConfiguration : IEntityTypeConfiguration<Post_PostTag>
{
    public const string TableName = "SimpleBlog_Post_PostTags";
    public void Configure(EntityTypeBuilder<Post_PostTag> builder)
    {
        builder.ToTable(TableName);
        builder.HasKey(p => p.Id);

        builder.HasOne(x => x.Post).WithMany(x => x.Tags).HasForeignKey(x => x.PostId).IsRequired();
        builder.HasOne(x => x.PostTag).WithMany(x => x.Posts).HasForeignKey(x => x.PostTagId).IsRequired();
    }
}
