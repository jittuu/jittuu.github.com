---
layout: post
title: "Comparing Cloud providers for new product development"
---

Choosing the Cloud is more difficult now as vendors regularly drop prices and offer new features. I'm sure there is no clear winner and each will have each own strength and weakness. So, it is better to set the context before we compare. 

# Context

- Team is small but all are seasoned developer.
- Going to release multiple times per day.
- At least two environments. Dev, Production.
- At least need to develop one web application and one mobile app.
- PaaS is always preferred than IaaS.
- Only AWS, Azure or Google.
- Main development platform will be either .Net Core or Golang.

# Basic Architecture

![image](https://cloud.githubusercontent.com/assets/168965/15355285/cb3c9500-1d17-11e6-956c-c7976705e036.png)

This is very typical architecture for public facing web application. Of course, it could be split into many micro services - it would be multiple API services calling each other.I think this diagram is good enought for the very high level architecture.

The following table describes the components and related cloud features.

|            | AWS               | Azure                | Google Cloud Platform    |
|------------|-------------------|----------------------|--------------------------|
| Web API    | Elastic Beanstalk | Azure WebSites       | AppEngine                |
| Database   | Amazon RDS        | Azure SQL            | Datastore/Cloud SQL      |
| Storage    | Amazon S3         | Azure Storage        | Cloud Storage            |
| Jobs       | Amazon Lambda     | Azure Functions      | AppEngine/Cloud Functions|
| CI         | third-party       | VsTeam Services      | third-party              |

## Web API

|            | Elastic Beanstalk | Azure WebSites       | AppEngine                |
|------------|-------------------|----------------------|--------------------------|
| Managed    | 3                 |  4                   | **5**                    |
| Price*     | $70.08            | $204                 | **$25.55**               |

Managed services mean server is maintained by provider. The higher number, the better. I gave Azure WebSites to 4 because it doesn't have other features compare to AppEngine such as version splitting, no downtime deployment, no free centralize cache (memcache/redis), etc.

*_price is estimated for two environments_

- AWS: 1 year no-upfront (1x t2.small, 1x t2.medium)
- Azure: (1x B1, 1x S2)
- AppEngine: (1x free quota, 1x n1-standard-1 flex-vm) _assuming that we got more traffic than what free quota provides._

## Database

| Database   | Amazon RDS        | Azure SQL            | Datastore/Cloud SQL      |
|------------|-------------------|----------------------|--------------------------|
| Price      | $77.38            | $79.98               | **$59.65**               |

- AWS: (1x db.t2.micro, 1x db.m3.medium)
- Azure: (1x B, 1x S2) _it is very difficult to convert DTU to server. So, I just guess it from [this review](https://cbailiss.wordpress.com/2015/01/31/azure-sql-database-v12-ga-performance-inc-cpu-benchmaring/)._
- Cloud SQL: (1x f1.micro - share CPU, 0.6 GB, 1x n1-standard-1 - 1CPU, 3.75 GB)

## Storage

The base line is
- 10 GB storage 
- PUT/LIST ops 1million
- GET object ops 10million
- 1TB Egress bandwidth (South-east-asia)

| Storage    | Amazon S3         | Azure Storage        | Cloud Storage            |
|------------|-------------------|----------------------|--------------------------|
| Price      | $130.21           | $141.93              | **$122.88**              |

## Jobs

For our usecase, there won't be many jobs running and we could re-use resource from Web such as WebJobs or AppEngine tasks. So, I'll skip the comparison.

## CI

We could use [CircleCI](https://circleci.com) (1 concurrent build is free) for AWS and Google while we use Visual Studio Team Services for Azure. So, I'll also skip it.

# Process 

I will try to break down into a few software development processes so that we can compare each process for different cloud providers.

![image](https://cloud.githubusercontent.com/assets/168965/15355311/e3d5d306-1d17-11e6-8292-7a059807b5fa.png)

Yes. This is SDLC for our team. We are targeting to release multiple times per day. Small, fast and incremental releases.

The following table contains some components which hasn't covered by the basic architecture components.

|            | AWS               | Azure                | Google Cloud Platform    |
|------------|-------------------|----------------------|--------------------------|
| Coding     | Go                | dotnet               | Go                       |
| Deployment | CodeDeploy        | **VsTeam Service**   | Deployment Manager       |
| Monitoring | third-party       | Application Insights | **StackDrivers**         |

## Coding

Both Go and dotnet core are great. It is all up to your team. Even if your team know about dotnet, the _new_ dotnet is completely new. You have to re-learn a lot. I warn you. :smile:

## Deployment

Visual Studio Team services included everything from CI to deployment and it is well integrate with Azure. I think it has some edge over other providers.

## Monitoring.

There are only two contenders, Azure and Google Cloud Platform. I didn't have good experience with Application Insights - to be fair it is still in preview.

- It is no stable yet ([7 incidents on May alone](https://blogs.msdn.microsoft.com/applicationinsights-status/))
- The portal is **slow**
- No API (to read data) yet.
- Very difficult to check trace log compare to [StackDriver Logging](https://cloud.google.com/logging/)
- No Detailed Tracing (Performance Monitoring) compare to [StackDriver Tracing](https://cloud.google.com/trace/)
- No centralize Error Reporting compare to [StackDriver Error Reporting](https://cloud.google.com/error-reporting/) 

# Container

How about Container?

Container has been very porpular in recent years and many company bet on that as future software packing. Even Microsoft partner with Docker to bring Docker to Windows (Nano Server). It won't be too long for Microsoft own Container Service will land to Azure. Anyway, for now - there is only two horses (AWS and GCP) in this race.

I personally haven't tried but both of them are just sit on top of VMs. If comparing VMs, **Google Cloud Platform** has some edge on that and [their container engine](https://cloud.google.com/container-engine/) is [K8](http://kubernetes.io/) is opensource - that will bring portability if you need to host in on-premise servers.

# BizSpark

You may notice that Azure is a bit more expensive than others but they have nice _BizSpark_ program. With that, you will get $150/month credit for **3 years**.

That will help you to run without spending money for **3 years** but after that you **still need to pay**.

# Conclusion

If you have reach this far, I believe you notice that I'm a bit toward Google Cloud Platform because of their pricing model (Per-Minute Billing and Automatic Discounts) and their PaaS offering such as AppEngine, StackDriver and Container Engine.

But. You will never wrong for choosing one because each has their own strength. The real pros and cons is depends on YOU. (Your Team, Skillset, Business deal/startup accelerator, etc.).

The best way to choose is draw your architecture and do some research on that with all three providers.