using SQLite;

namespace GunRush.Models;
[Table("run")]
public class Run
{
    [PrimaryKey, AutoIncrement]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("seed")]
    public int Seed{ get; set; }
    
    [Column("difficulty")]
    public int Difficulty { get; set; }
    
    [Column("duration")]
    public decimal Duration { get; set; }
    
    [Column("kills")]
    public int Kills { get; set; }
    
    [Column("won")]
    public bool Won { get; set; }
}