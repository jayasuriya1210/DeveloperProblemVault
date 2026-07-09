const form = document.querySelector("form");

form.addEventListener("submit", async function (event) {

    event.preventDefault();

    const issue = {

        issueTitle:  document.querySelectorAll("input")[0].value,
        projectName: document.querySelectorAll("input")[1].value,
        status:      document.querySelectorAll("select")[0].value,
        priority:    document.querySelectorAll("select")[1].value,
        description: document.querySelectorAll("textarea")[0].value,
        rootCause:   document.querySelectorAll("textarea")[1].value,
        solution:    document.querySelectorAll("textarea")[2].value

    };

    await addIssue(issue);

    alert("Issue Added Successfully!");

    form.reset();

});