# Department traversal library

A mysterious project, a library for tracking every movement of Vasya, who set out on a quest to get through a very, very bureaucratic organization.

## Usage

The code is supposed to be pretty straightforward if you know the task, but here's the gist of it :)

Use `DepartmentArrangementBuilder` to arrange departments before building the arrangement.
The configuration of department's actions is done through `DepartmentWorker`. 
Don't forget to stay within the bounds you've set for yourself and set the starting and finishing departments.

For example, to set up a department #5 that stamps the 1st stamp, unstamps the 2nd stamp, and redirects Vasya to the 3rd department, you'd do this:
```c#
var dabuilder = ...;

// some setup

dabuilder.SetSimpleDepartment(5, new DepartmentWorker(1, 2, 3));

// some more setup

var arrangement = dabuilder.Build();
```

Once you're done building the arrangement, just query it using `DepartmentArrangement.Query` and get all you need to know and more about Vasya. How evil.
