***Backend _.Net_ Interview Coding Exercise***

> Build a solution to solve the below problem using a .Net 4.6 solution.

**Problem:**
  1. We need an API endpoint that has the ability take a request to scrape a web page.
  2. The API must allow for submitting a job, checking the status of a job, and retrieving the results.
  3. This endpoint will be hit very heavily, so we need to design it to remain available under heavy load and when a scraping job takes an extended time.



**Hints:**
  1. Look at using a job scheduler like Quartz
  2. Be sure to write unit tests for different cases...
  3. Concurrency with multiple jobs running.

**Bonus:**
  1. Solve this issue without using a database.
  2. Don't use any third party web scraping frameworks.
  3. Think how this API will be consumed and what you might suggest to improve this.
  4. Documentation & Local repo.
