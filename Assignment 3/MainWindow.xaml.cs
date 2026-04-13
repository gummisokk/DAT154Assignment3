using System.Data.Entity;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Assignment_3.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.Tokens;

namespace Assignment_3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly Dat154Context dx = new();

        private readonly LocalView<Student> Students;
        private readonly LocalView<Course> Courses;
        private readonly LocalView<Grade> Grades;

        private string _searchName = "";
        private Course? _searchCourse = null;
        private string _searchGrade = "";
        private bool _searchStryk = false;

        private void ApplySearch()
        {
            if (_searchCourse != null || _searchGrade != null || _searchStryk)
            {
                studentList.Visibility = Visibility.Hidden;
                gradeList.Visibility = Visibility.Visible;
                gradeList.DataContext = Grades
                    .Where(g => _searchCourse == null || g.CoursecodeNavigation == _searchCourse)
                    .Where(g => _searchGrade.IsNullOrEmpty() || g.Grade1.CompareTo(_searchGrade) <= 0)
                    .Where(g => !_searchStryk || g.Grade1 == "F")
                    .Where(g => g.Student.Studentname.Contains(_searchName))
                    .OrderBy(g => g.Student.Studentname);
                return;
            }

            studentList.Visibility = Visibility.Visible;
            gradeList.Visibility = Visibility.Hidden;
            studentList.DataContext = Students?
                .Where(s => s.Studentname.Contains(_searchName))
                .OrderBy(s => s.Studentname);
        }

        public MainWindow()
        {
            InitializeComponent();

            Students = dx.Students.Local;
            Courses = dx.Courses.Local;
            Grades = dx.Grades.Local;

            dx.Students.Load();
            dx.Courses.Load();
            dx.Grades.Load();

            inGrade.DataContext = Grades.Select(g=>g.Grade1).Distinct().OrderBy(g => g);
            inCourse.DataContext = Courses.OrderBy(c => c.Coursename);
            studentList.DataContext = Students.OrderBy(s => s.Studentname);
        }

        private void TextBox_SearchName(object sender, TextChangedEventArgs e)
        {
            _searchName = (sender as TextBox)?.Text ?? "";
            ApplySearch();
        }

        private void ComboBox_SearchCourse(object sender, SelectionChangedEventArgs e)
        {
            _searchCourse = (sender as ComboBox)?.SelectedItem as Course;
            ApplySearch();
        }
        private void ComboBox_SearchGrade(object sender, SelectionChangedEventArgs e)
        {
            _searchGrade = (sender as ComboBox)?.SelectedItem as string ?? "";
            ApplySearch();
        }

        private void CheckBox_Stryk(object sender, RoutedEventArgs e)
        {
            _searchStryk = (sender as CheckBox)?.IsChecked ?? false;
            ApplySearch();
        }
    }
}