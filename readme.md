# RESTful dotnet Core API 

#### Basic Steps / Content: 
- Create api
- Setup database connection
- Make repositories + DTO's
- Create controllers + check gets / posts etc work in postman
- Install Swashbuckle + Swagger + Add XML comments
- Authenticate with admin user

api means "Application Programming Interface" => a software intermediary that allows two applications to talk to each other

An API works via request and response

### The request object
verb
headers
content

HTTP Verbs / actions:
 - GET: fetches/requests 
 - POST: creates/inserts resource
 - PUT: updates/modifies resource
 - PATCH: updates/modifies *partial* resource (use if you only want to update part of the resource)
 - DELETE: deletes/removes resource
 - more verbs..(not often used)

Headers:
name-value pairs that are meta data about the request
 - Content type: Content's format
 - Content Length: size of the content
 - Authorization: who is making the request
 - Accept: what are the accepted type(s)
 - more headers... 

Content:
This could be anything
  - HTML, CSS, XML, JSON
  - Information for the request
  - Blobs etc

### The response object
status code
headers
content

Status Codes: 
  - 100-199 => informational
  - 200-299 => success
    - 200 - OK
    - 201 - Created
    - 204 - No Content
  - 300-399 => redirection 
  - 400-499 => Client Errors
    - 400 - bad request
    - 404 - not found
    - 409 - conflict
  - 500-599 => server errors
    - 500 - internal server error

Headers: 
response's metadata
  - Content type: contents format
  - Content length: size of the content
  - Expires: when is this invalid
  - more headers...

Content:
  - HTML, CSS, XML, JSON
  - Blobs etc.

## Creating project
mkdir fileName
cd fileName
dotnet new webapi

on side menu on the > icon, click and then click "run" with .NET Core selected

===


#### Create project: ``dotnet new webapi``

#### Create model: New folder in root "Models" => make new file NationalPark.cs & fill it out

#### Setup database connection: 

(tutorial uses sql server, i needed to run it on docker for mac)

  - get a database setup in docker: 
    - ``sudo docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=reallyStrongPwd123" -p 1448:1448 --name sqlDb2 -d microsoft/mssql-server-linux``

  - in appsettings.json: 
  ````
    "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1444;Database=Parky2;User Id=SA;Password=reallyStrongPwd123;"
  },
  ````
  - create new folder in root "Data" => make new file ApplicationDbContext.cs
    - when inheriting from DbContext need to ``dotnet add package Microsoft.EntityFrameworkCore.SqlServer``
    - fill out ApplicationDbContext.cs

  - in startup.cs in configureservices, add: ``sservices.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));`` (this looks inside appsettings.json for whats under "defaultConnection"!)
    - to usesqlserver, need to ``dotnet add package microsoft.entityframeworkcore.sqlserver``

  - run ``dotnet add package Microsoft.EntityFrameworkCore.Tools``
  - to start creating db, run ``dotnet ef migrations add NationalPark`` (similar to rails, makes a migration!)
  - run ``dotnet ef database update``

## To get it running
Note: Had to use docker as mysql is windows or linux only. 
- ``docker ps`` to check if docker container is running, if it isnt: ``docker start sql_server_dotnetAPI_demo``
- to see inside database type: ``mssql -d 'Parky' -u sa -p reallyStrongPwd123`` (where 'Parky' is the name of the database)
- to perform a query, type: ``SELECT * FROM NationalParks`` (sql stuff)




to install a package, need to type in the console: ``"dotnet add package Microsoft.EntityFrameworkCore"`` for example, which will add it to the csproj file.

to fix red underlined stuff, click on it then go command + . 

===


DTO's: 
Domain model:
National Park
- ID
- Name
- State
- Established date
- Image
- Created date
- Updated date

Create DTO:
- Name
- State
- Established date
- Image
(wont be an id/created date/updated date, auto generated by database)

Update DTO:
- ID
- Name
- State
- Established date
- Image

===

## API Documentation 

Adds the following libraries:
SwashBuckle.AspNetCore.Swagger
SwashBuckle.AspNetCore.SwaggerGen
SwashBuckle.AspNetCore.SwaggerUi

NOTE: Adding SwashBuckle.AspNetCore gets all the above 3 by default!
``dotnet add package SwashBuckle.AspNetCore``

In startup.cs needed to add this to ConfigureServices:
````cs
  services.AddSwaggerGen(options => {
      options.SwaggerDoc("ParkyOpenAPISpec",
          new Microsoft.OpenApi.Models.OpenApiInfo() {
              Title = "Parky API",
              Version = "1"
          });
  });
````
and this to Configure: 
````cs
app.UseSwagger();
app.UseSwaggerUI(options => {
  options.SwaggerEndPoint("/swagger/ParkyOpenAPISpec/swagger.json", "Parky API");
});
````

to check swagger stuff go to: 
https://localhost:5001/swagger/ParkyOpenAPISpec/swagger.json
"info" shows the info declared in the openApiInfo set in ConfigureServices
"paths" shows all the routes that our API exposes. Note responses are incomplete / inaccurate

and this to Configure: 
````cs
app.UseSwaggerUI(options => {
  options.SwaggerEndPoint("/swagger/ParkyOpenAPISpec/swagger.json", "Parky API");
  options.RoutePrefix = ""; //helps to make /swagger the default route
});
````
to check swagger stuff now at: 
https://localhost:5001/swagger/
=> Can test api's from the above link! Works like postman.

### Improving documentation with XML comments

"Extensible Markup Language (XML) is a markup language that defines a set of rules for encoding documents in a format that is both human-readable and machine-readable"

Add more info:
````cs
/// <summary>
/// Get individual national park
/// </summary>
/// <param name="nationalParkId"> The Id of the national Park </param>
/// <returns></returns>
[HttpGet("{id:int}", Name = "GetNationalPark")]
public IActionResult GetNationalPark(int id)
{
  var obj = _npRepository.GetNationalPark(id);
  if (obj == null)
  {
    return NotFound();
  }

  var objDto = _mapper.Map<NationalParkDto>(obj);
  return Ok(objDto);
}
````

in csproj file add something like: 

````
<PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|AnyCPU'">
  <DocumentationFile>bin\Debug\$(TargetFramework)\$(MSBuildProjectName).xml</DocumentationFile>
  <NoWarn>1701;1702;1705;1591</NoWarn>
</PropertyGroup>

//i.e:
  <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|AnyCPU'">
    <DocumentationFile>bin\Debug\netcoreapp3.1\ParkyAPI.xml</DocumentationFile>
    <NoWarn>1701;1702;1705;1591</NoWarn>
  </PropertyGroup>
````

Need to update swagger doc code: 
````cs
services.AddSwaggerGen(options => {
    options.SwaggerDoc("ParkyOpenAPISpec",
        new Microsoft.OpenApi.Models.OpenApiInfo() {
            Title = "Parky API",
            Version = "1"
        });
    var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var cmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
    options.IncludeXmlComments(cmlCommentsFullPath);
});
````

To fix up response types in swagger, need to add "producesResponseType", like so: 
````cs
/// <summary>
    /// Get list of national parks.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(List<NationalParkDto>))] //need to add this (returning Ok() is a 200 response type)
    [ProducesResponseType(400)] // and this (auto handled by dotnet core)
    public IActionResult GetNationalParks()
    {
      var objList = _npRepository.GetNationalParks();
      var objDto = new List<NationalParkDto>();

      foreach(var obj in objList) {
        objDto.Add(_mapper.Map<NationalParkDto>(obj));
      }

      return Ok(objDto);
    }
````

````cs
[ApiController]
  [Route("api/[controller]")]
  [ProducesResponseType(StatusCodes.Status400BadRequest)] //since 404 is generic accross all routes, can add this to the top before the class!
  public class NationalParksController : ControllerBase
  {
    private readonly INationalParkRepository _npRepository;
    private ...
````


===

### Order for adding trails

1. Add model to models file: 
create Tails.cs => fill out class file
2. Add dto to models=>dtos file:
3. Add trail repository 
4. Add trail interface (ITrailRepository)
5. Add migration... ``dotnet ef migrations add AddTrailToDb`` and ``dotnet ef database update``

What is a repository?
"Conceptually, a Repository encapsulates the set of objects persisted in a data store and the operations performed over them, providing a more object-oriented view of the persistence layer"
(In this project, it seperates out some logic from the controller)

===

### Versioning

For versioning, make a copy of a controller and call if V2

need to install two new packages: ``dotnet add package Microsoft.AspNetCore.Mvc.Versioning`` and ``dotnet add package Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer``

in startup.cs add 
````cs
services.AddApiVersioning(options => 
{
  options.AssumeDefaultVersionWhenUnspecified = true;
  options.DefaultApiVersion = new ApiVersion(1, 0);
  options.ReportApiVersions = true;
});
````

=== 

### changing the database tables

- in the model, add a field (and DTO too)
- start a new migration: ``dotnet ef migrations add NationalPark`` and ``dotnet ef database update``



### Authorization 

For the API inheriting from Controller is what you do when using MVC, but optimal is inheriting from ControllerBase

In the controller, for one of the methods add ``[Authorize]`` 

Now when you hit the api, it doesn't work because you're not authorized...
Enable users to be authorized => create a user.cs model
run migrations / update
create IUserRepository and UserRepository

General flow: 
=> Add model "User"
=> Add iRepository "IUserRepository"
=> Add repository "UserRepository"
=> In startup.cs add ``services.AddScoped<IUserRepository, UserRepository>();``

Secret Key / Bearer:
=> Need to configure secret key when dealing with tokens, add to appsettings.json!
=> Create a new cs folder in root, needs to be called "AppSettings" (what it was called in the appsettings json file), and get/set the key inside (called "Secret" in json file)!
=> In startup.cs add ``services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));``
=> need to ``dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer``
=> need to add the following to startup.cs configureServices: 
````cs
var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection); //this will configure the AppSettings class with whatever we put in appsettings.json!

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret); //gets the string out of json file

            services.AddAuthentication(x => 
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x => {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
````
=> need to add the following to startup.cs configure pipeline: 
````cs
 app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
);
app.UseAuthentication(); //need to add authentication before authorization. order matters in this pipeline
app.UseAuthorization();
````
=> also need to add services.AddCors(); (he adds it to the top!)

=> used sql commands to make new user id 1, "1", "1", "1"


Assigning [Authorization(Role="Admim")] :
When a user signs up, we can assign their role = "Admin" like so: 
````cs
public User Register(string username, string password)
    {
      User userObj = new User()
      {
        Username = username,
        Password = password,
        Role = "Admin"
      };

      _db.Users.Add(userObj);
      _db.SaveChanges();
      userObj.Password = "********";
      return userObj;
    }
````
HOWEVER, the API auth is based in the token. So we have to let the token know the user as this role. 
````cs
   var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[]{
          new Claim(ClaimTypes.Name, user.Id.ToString()),
          new Claim(ClaimTypes.Role, user.Role) //this line here does this!
        }),
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };
````