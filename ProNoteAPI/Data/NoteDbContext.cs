using Microsoft.EntityFrameworkCore;
using ProNoteAPI.Models.Domain;

namespace ProNoteAPI.Data
{
    public class NoteDbContext : DbContext
    {
        public NoteDbContext(DbContextOptions<NoteDbContext> options)
            : base(options)
        {


        }

        public DbSet<Note> Notes { get; set; }

    }




}
