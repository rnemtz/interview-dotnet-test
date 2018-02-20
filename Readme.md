***Backend _.Net_ Interview Coding Exercise***

> Build a solution to solve the below problem using a .Net 4.6.1 (or later) or a .NET Core 2.0 (or later) solution.

**Problem:**
  1. We need a RESTful API that has the ability take a request to asynchronously scrape a web page for all the links.
  2. The API must allow for submitting a job, checking the status of a job, and retrieving the results.
  3. This endpoint will be hit very heavily, so we need to design it to remain available under heavy load and when a scraping job takes an extended time.
  4. You should provide good documentation and your README should make it clear how to work with your API.



**Hints:**
  1. Be sure to include some form of queuing mechanism for managing the scraping job.
  2. Concurrency with multiple jobs running.
  3. Be sure to write unit tests for different cases.
  4. Your solution must compile.
  5. Your solution should be easy to get up and running.

**Bonus:**
  1. Solve this issue without using a database.
  2. Think how this API will be consumed and document what you might suggest to improve it.
  3. Provide API documentation via swagger or something like it.
