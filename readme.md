# RESTful dotnet Core API 

api means "Application Programming Interface" => a software intermediary that allows two applications to talk to each other

It works via request and response

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
mkdir <filename>
cd <filename>
dotnet new webapi

