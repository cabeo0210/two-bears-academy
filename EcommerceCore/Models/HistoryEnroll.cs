using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceCore.Models;

public class HistoryEnroll:BaseClass
{
    [Key]
    public int HistoryEnrollId { get; set; }
    public int EnrollId { get; set; }
    [ForeignKey(nameof(EnrollId))]
    public Enroll Enroll { get; set; }
    public int StatusEnroll { get; set; }
}