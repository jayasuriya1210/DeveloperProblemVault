const id = new URLSearchParams(window.location.search).get("id");

async function loadIssue() {
    const response = await getIssue(id);
    const issue = response.result;

    document.getElementById("issueTitle").value  = issue.issueTitle;
    document.getElementById("projectName").value = issue.projectName;
    document.getElementById("status").value      = issue.status;
    document.getElementById("priority").value    = issue.priority;
    document.getElementById("description").value = issue.description;
    document.getElementById("rootCause").value   = issue.rootCause;
    document.getElementById("solution").value    = issue.solution;
}

document.querySelector("form").addEventListener("submit", async function (event) {
    event.preventDefault();

    const issue = {
        issueTitle:  document.getElementById("issueTitle").value,
        projectName: document.getElementById("projectName").value,
        status:      document.getElementById("status").value,
        priority:    document.getElementById("priority").value,
        description: document.getElementById("description").value,
        rootCause:   document.getElementById("rootCause").value,
        solution:    document.getElementById("solution").value
    };

    await updateIssue(id, issue);
    alert("Issue updated successfully!");
    location.href = "view-issues.html";
});

loadIssue();
