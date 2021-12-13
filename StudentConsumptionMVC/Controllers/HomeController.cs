using Microsoft.AspNetCore.Mvc;
using StudentConsumptionMVC.Models;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace StudentConsumptionMVC.Controllers
{
    public class HomeController : Controller
    {
        public StudentService.ServiceClient Client = new StudentService.ServiceClient();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public DataSet ToDataSet(StudentService.ArrayOfXElement arrayOfXElement)
        {
            var strSchema = arrayOfXElement.Nodes[0].ToString();
            var strData = arrayOfXElement.Nodes[1].ToString();
            var strXml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n\t<DataSet>";
            strXml += strSchema + strData;
            strXml += "</DataSet>";

            DataSet ds = new DataSet("TestDataSet");
            ds.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(strXml)));

            return ds;
        }

        public IActionResult Index(string search)
        {
            StudentService.Student student1 = new StudentService.Student();
            List<Student> students = new List<Student>();
            DataSet AllStudents;
            if (search != null)
            {
                student1.StdID = search;
                AllStudents = ToDataSet(Client.SearchStudentsRecord(student1));
            }
            else
            {
                AllStudents = ToDataSet(Client.GetStudentsRecords());
            }
            foreach (DataRow row in AllStudents.Tables[0].Rows) {
                Student student = new Student();
                student.Stdid = row.ItemArray[0].ToString();
                student.Name = row.ItemArray[1].ToString();
                student.Email = row.ItemArray[2].ToString();
                student.PhoneNumber = row.ItemArray[3].ToString();
                student.Gender = row.ItemArray[4].ToString();
                students.Add(student);
            }

                return View(students);
        }

        public ActionResult Add()  
        {  
            return View();  
        }  
        [HttpPost]  
        public ActionResult Add(Student mdl)  
        {

            StudentService.Student std = new StudentService.Student();
            std.StdID = mdl.Stdid;
            std.Name = mdl.Name;
            std.Email = mdl.Email;
            std.Phone = mdl.PhoneNumber;
            std.Gender = mdl.Gender;
            Client.AddStudentsRecord(std); 
            return RedirectToAction("Index", "Home");  
            
        }  

        [HttpPost]
        public ActionResult Edit(Student mdl)
        {
            StudentService.Student std = new StudentService.Student();
            std.StdID = mdl.Stdid;
            std.Email = mdl.Email;
            std.Phone = mdl.PhoneNumber;
            Client.UpdateStudentsContact(std);

            return RedirectToAction("Index", "Home");
        }
        public ActionResult Edit()
        {
            return View();
        }
        public ActionResult Delete(string id)  
        {
            StudentService.Student std = new StudentService.Student();
            std.StdID = id;
            Client.DeleteRecords(std);
            
                return RedirectToAction("Index", "Home");  
            
        } 

        public ActionResult SearchResult(string id)
        {
            StudentService.Student student1 = new StudentService.Student();
            student1.StdID = id;
            List<Student> students = new List<Student>();
            DataSet AllStudents = ToDataSet(Client.SearchStudentsRecord(student1));

            foreach (DataRow row in AllStudents.Tables[0].Rows)
            {
                Student student = new Student();
                student.Stdid = row.ItemArray[0].ToString();
                student.Name = row.ItemArray[1].ToString();
                student.Email = row.ItemArray[2].ToString();
                student.PhoneNumber = row.ItemArray[3].ToString();
                student.Gender = row.ItemArray[4].ToString();
                students.Add(student);
            }
            

            return View(students);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}