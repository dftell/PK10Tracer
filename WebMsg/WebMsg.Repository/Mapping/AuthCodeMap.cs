using WebMsg.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebMsg.Repository.Mapping
{
    /// <summary>
    /// table应用配置
    /// </summary>
    public class AuthCodeMap : IEntityTypeConfiguration<AuthCode>
    {
        public void Configure(EntityTypeBuilder<AuthCode> builder)
        {
            builder.HasKey(p => p.PK_ACID);
            builder.Property(t => t.PK_ACID).HasMaxLength(64).HasColumnName("PK_ACID");
            builder.Property(t => t.Phone).HasMaxLength(24).HasColumnName("Phone");
            builder.Property(t => t.Code).HasMaxLength(8).HasColumnName("Code");
            builder.Property(t => t.CType).HasColumnName("CType");
            builder.Property(t => t.Sort).HasColumnName("Sort");
            builder.Property(t => t.IsRemove).HasColumnName("IsRemove");
            builder.Property(t => t.UpdateTime).HasColumnName("UpdateTime");
            builder.Property(t => t.CreateTime).HasColumnName("CreateTime");
            builder.ToTable("AuthCode");
        }
    }
}
