namespace pragim_dotNetWebApp.Models {
  public class Employee {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public Dept Department { get; set; }
  }
}
