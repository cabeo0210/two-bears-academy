using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace EcommerceCore.Models;

public class Enroll :BaseClass
{
    [Key]
    public int EnrollId { get; set; }
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    public int LeadId { get; set; }
    [ForeignKey(nameof(LeadId))]
    public Lead Lead { get; set; }
    [JsonIgnore]
    public List<HistoryEnroll> HistoryEnrolls { get; set; }
}