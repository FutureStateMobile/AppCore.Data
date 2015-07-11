FutureState.AppCore.Data
============

A Portable, cross-platform, light weight, opinionated ORM designed to work across multiple Databases 

----

###Why another ORM, aren't there already a ton out there?###

In our attempt to create our idea of the ultimate sync engine between mobile devices and a central server, our research lead us to projects that just didn't "feel right". We found many projects that worked across all platforms for a specific database ([Sqlite.Net](https://github.com/praeclarum/sqlite-net) for example), but none of them had the added benefit of working across database engines. It was at that time that we decided that what we needed was a way to communicate with pretty much any ADO database from within a Portable Class Library, allowing us to not only use shared code on both the Client and the Server, but also use shared code across the individual databases themselves. It's out of this desire that ***FutureState.AppCore.Data*** was conceived.

----

###Current Support###

*note: we take pull requests if you'd like to support more ;)* 


| Platform | Sqlite | SQL Server (2012) | PostgreSQL | MySQL  |
| :––––––– | –––––: | –––––––––––-----: | –––––––––: |–––––-: |
| Windows       | yes    |  yes              | coming     | coming |


| Android       | yes    |  no               | no         | no     |
| iOS           | yes    |  no               | no         | no     |
| Windows Phone | coming |  no               | no         | no     |

###That sounds good, how do I use it?###

We're working on building out the [Wiki](https://github.com/FutureStateMobile/AppCore.Data/wiki), so more information will be found there.

To get started, can either compile the project yourself. Simply clone the project to your Windows computer (we're currently using MSBuild and Powershell for our builds), and run the `build.ps1` script. From there you can either grab the dll's out of the `build-artifacts\output` directory, or scoop the nupkg out of the `build-artifacts` directory and drop it in your local nuget package source. 

Or you can just grab it from Nuget.org

    > Install-Package FutureState.AppCore.Data

To give you a taste of what it looks like, here are some examples of CRUD operations.

**Create**

    var jill = new StudentModel
        {
            Id = StudentJillId,
            FirstName = "Jill",
            LastName = "",
            Email = JillEmail,
            Courses = new List<CourseModel> { englishCourse },
        };

    _dbProvider.Create( jill );

**Read**

    IList<StudentModel> allStudents = _dbProvider.Query<StudentModel>().Select().ToList();
    StudentModel jill = _dbProvider.Query<StudentModel>().Where(s => s.Email == "JillEmail").Select().FirstOrDefault();

**Update**

    jill.Email = "JillNewEmail";
    _dbProvider.Update<StudentModel>(jill);

**Delete**  
*note: this is still a work in progress*

    // first flavor
    _dbProvider.Query<StudentModel>().Where( s => s.Email == "JillNewEmail" ).Delete();

    // second flavor
    _dbProvider.Delete<StudentModel>(s => s.Email == "JillNewEmail");

###Dependencies###

Since this ORM is simply a wrapper over an ADO database, it doesn't contain any drivers for the specific databases. If you're using SQLite, you'll want to use the Mono.Data.Sqlite library located in the [3rd party/Mono.Data.Sqlite](https://github.com/FutureStateMobile/AppCore.Data/tree/master/3rd%20party/Mono.Data.SQLite.1.0.61.0) directory.

Also, the project was built using Xamarin. We haven't had a chance to investigate compatibility in a Xamarin Free Environment.

###Meta###

We absolutely welcome contributions/suggestions/bug reports from you (the people using this package). If you have any ideas on how to improve it, please post an [issue](https://github.com/FutureStateMobile/AppCore.Data/issues) with steps to reproduce, or better yet, submit a Pull Request.

###License###

We offer 2 license choices:

1. [Microsoft Public License (Ms-PL)](http://www.microsoft.com/en-us/openness/licenses.aspx#MPL)
2. Commercial Licenses are also available, please email sales@futurestatemobile.com for more information.
