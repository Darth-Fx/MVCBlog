using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using MVCBlog.Core.Database;

namespace MVCBlog.Core.Commands
{
    public class DeleteBlogEntryCommandHandler : ICommandHandler<DeleteBlogEntryCommand>
    {
        private readonly IRepository repository;

        public DeleteBlogEntryCommandHandler(IRepository repository)
        {
            this.repository = repository;
        }

        public async Task HandleAsync(DeleteBlogEntryCommand command)
        {
             var entity = await this.repository.BlogEntries
                .Include(b => b.BlogEntryFiles).Include(c => c.Tags)
                .SingleOrDefaultAsync(e => e.Id == command.Id);

            if (entity != null)
            {
                var tagsToRemove = entity.Tags
                  .ToArray();
                foreach (var tag in tagsToRemove)
                {
                    repository.Tags.Remove(tag);
                }
                foreach (var blogEntryFile in entity.BlogEntryFiles)
                {
                    blogEntryFile.DeleteData();
                }

                this.repository.BlogEntries.Remove(entity);

                await this.repository.SaveChangesAsync();
            }
        }
    }
}
