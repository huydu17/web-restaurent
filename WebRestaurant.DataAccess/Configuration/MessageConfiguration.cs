using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebRestaurant.Models;

namespace WebRestaurant.DataAccess.Configuration
{
    internal class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Message");
            builder.HasKey(x => x.Id);
            builder.Property(x=>x.Content).IsRequired().HasMaxLength(1024);
            builder.HasOne(m => m.AppSender)
              .WithMany(x=>x.Messages)
              .HasForeignKey(m => m.SenderId)
              .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(m => m.Conversation)
              .WithMany(x=>x.Messages)
              .HasForeignKey(m => m.ConversationId)
              .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
