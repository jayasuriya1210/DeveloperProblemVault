async function loadIssues() {

    const response = await getAllIssues();

    const table = document.getElementById("issueTable");

    table.innerHTML = "";

    response.result.forEach(issue => {

        table.innerHTML += `

        <tr>

            <td>${issue.id}</td>

            <td>${issue.issueTitle}</td>

            <td>${issue.projectName}</td>

            <td>${issue.status}</td>

            <td>${issue.priority}</td>

            <td>

                <a href="issue-details.html?id=${issue.id}">View</a> |
                <a href="update-issue.html?id=${issue.id}">Edit</a> |
                <a href="#" onclick="handleDelete(${issue.id})">Delete</a>

            </td>

        </tr>

        `;

    });

}

loadIssues();

async function handleDelete(id) {
    if (confirm("Are you sure you want to delete this issue?")) {
        await deleteIssue(id);
        loadIssues();
    }
}