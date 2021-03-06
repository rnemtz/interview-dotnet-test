
# EAZECRAWLER

RESTful API that allows the client to request to scrape web pages. 

When a request to scrap a web page is received, it goes straight to an event queue where it fires up a process request to the Scheduler component using Crawler job to process links in the web page. It uses a Breadth-first approach to handle the links queue.

Crawler job instance will grab basic information from the web page content such as Title, Descripcion and store the URL itself to reference the job. Along with this information, it will get all its links and will add them to a queue to process them up to two level deep of the original URL. 

---
### API DOCUMENTATION

I have chosen Swashbuckle.AspNetCore a Swagger implementation for dot-net core 2.x.
You will find id at the root of the API:
http://eazecrawler.us-east-1.elasticbeanstalk.com

---
### ENDPOINTS

#### Post a new Job


Url: **http://eazecrawler.us-east-1.elasticbeanstalk.com/api/v1/scraper**

Method: **POST**

Headers:
 `ContentType: application/json`

Request body:
`{
"Name":"Google",
"Url": "https://www.google.com"
}`

#### Result
    OK    
    {
        "id": "4c74452e-8384-4f81-9cde-f3af5e919b1f",
        "name": "Google",
        "url": "https://www.google.com",
        "status": 0,
        "createdAt": "2018-10-07T22:23:36.9164581Z",
        "completedAt": "0001-01-01T00:00:00"
    }

#### Get all scraped urls

Url: **http://eazecrawler.us-east-1.elasticbeanstalk.com/api/v1/scraper**

Method:  **GET**

#### Result

    OK 
    [
        {
            "title": "Google",
            "description": "",
            "url": "https://www.google.com/"
        },
        {
            "title": "Google",
            "description": "",
            "url": "https://www.google.com/webhp?tab=ww"
        },
        ...
    ]

#### Delete Current Results

Url: **http://eazecrawler.us-east-1.elasticbeanstalk.com/api/v1/scraper**

Method: **DELETE**

#### Result
  
    OK 
    {
        "mesasge": "All results were deleted sucessfully",
        "deletedResults": 1,
        "statusCode": 200
    }

#### Get specific Job Information

Url: **http://eazecrawler.us-east-1.elasticbeanstalk.com/api/v1/scraper/{id}**

Method: **GET**

#### Result 
  
    OK
    {
        "jobDetail": {
            "id": "2bd555fb-673b-41d3-ae7e-71475f07dfe4",
            "name": "Google",
            "url": "https://www.google.com",
            "status": 2,
            "createdAt": "2018-10-09T02:42:13",
            "completedAt": "2018-10-09T02:42:29.965224Z"
        },
        "results": {
            "list": [
                {
                    "title": "Google",
                    "description": "",
                    "url": "https://www.google.com/"
                },
                {
                    "title": "Google - Apps",
                    "description": "",
                    "url": "http://www.google.com/mobile/?hl=en&tab=wD"
                }
            ]
        }
    }

#### Get specific Job results

Url: **http://eazecrawler.us-east-1.elasticbeanstalk.com/api/v1/scraper/{id}/results**

Method: **GET**

Result
        
    OK
    [
        {
            "title": "Google",
            "description": "",
            "url": "https://www.google.com/"
        },
        {
            "title": "Google - Apps",
            "description": "",
            "url": "http://www.google.com/mobile/?hl=en&tab=wD"
        }
    ]

---
### ARCHITECTURE

#### DEVOPS

The .net solution is written in dot-net Core 2.1 and prepared to run on a Docker Container or IIS Server. Currently, it's deployed to AWS-EBS.

URL: **http://eazecrawler.us-east-1.elasticbeanstalk.com/**

TYPE: **EBS Single host**

SIZE: **t2.micro**

OS: **IIS 10.0 running on 64bit Windows Server Core 2016/1.2.0**

![Infrastructure](https://s3.amazonaws.com/eaze-crawler-content/cloudcraft.JPG)

#### DEPENDENCIES   

![Project Dependencies](https://s3.amazonaws.com/eaze-crawler-content/Project+Dependancy.JPG)

**EazeCrawler**
This is where the controllers are defined and it will be the exposed to public methods.

**Services**
I have separated the actions in services
Scheduler. This service receives a request from the endpoint and schedules the job to run as soon as possible using a Crawler class instance. It also holds the methods that will communicate with EazeCrawler.Data and extract needed information regarding jobs and results.

Taking hints into account, I used Quartz as the job scheduler for the first time, I didn't rely upon all methods as I wanted to make my own implementation, so it was used only for scheduling and running jobs.

**Data**
This specific component, acts as the data repository, in this case, it's declared as a singleton, thread safe using concurrent collections to store all requested jobs and then a different collection that stores the results. The whole purpose of adding a different project for storing in-memory information is that this layer can be extracted and switch it to a different data repository without modifying any other layer's code.

**Common (Models)**
It stores the models and interfaces used among all other components/projects, you will find that all projects are making reference to this library.

EventManager. This singleton common service will fire up events to all subscribed components, it can be extended for any needed event. Since this is part of the common project, you will notice that it was not declared in the IOC as a singleton instance, the only reason is that this may be used in any other project and I didn't want to add IOC references to all projects, only needed.

---
### THIRD PARTY LIBRARIES

It uses third-party libraries for easier implementation of dependency injection, I used a .gitignore for .net core apps and HTML agility pack for dealing with HTTP components.

[Ninject Implementation for .Net Core](https://dev.to/cwetanow/wiring-up-ninject-with-aspnet-core-20-3hp)

[DotNetCore GitIgnore](https://github.com/thangchung/awesome-dotnet-core/blob/master/.gitignore)

[HTML Agility Pack](https://html-agility-pack.net)

[Swashbuckle (Swagger)](https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.1&tabs=visual-studio%2Cvisual-studio-xml)

---
### IMPROVEMENTS
There is a lot of room for improvement. I will list some ideas here:

- Implement Quartz scheduler with Redis so it will store all jobs in a different source than ram-store. This will allow the crawler to run in cluster mode, a very nice way for scaling and increase performance.
- Auto-Scale all EazeCrawler instances and set thresholds, this way we won't have to handle the growth, EBS will handle it for us.
- Implement authentication and security for API calls, currently is wide-open. One way would be with implementing .net identity and add sessions to the headers. Also, we can add access list and only allow requests from specific hosts.
- Implement a User Interface to trace scraped URLs, this will allow the user(s) to see posted jobs graphically if needed or just the crawler history and will allow also to start searching for content.
- Implement Elastic-Search or any other search for URLs and content.
- Implement ECS (In case AWS is the cloud solution) or Kubernetes with Docker containers if it's decided to go with services.

This might give you an idea of how this project can be extended.

### Unit Tests  

For unit testing, I selected the xUnit framework and Moq. The implementation of unit testing for EazeCrawler.Data are in the EazeCrawler.Tests project.

Unfortunately, I don't have much time to write all the unit tests, the intention was to use a TDD approach since the beginning, and I started the design and then code implementation while I waited for the recruiter response with my questions. Anyway, I don't want to extend the delivery of this code repository, and adding I will have a very busy week with code deployments at work, hence, unit tests incompleted.