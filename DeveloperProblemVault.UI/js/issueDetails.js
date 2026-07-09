const params = new URLSearchParams(window.location.search);

const id = params.get("id");

async function loadIssue() {

    const response = await getIssue(id);

    const issue = response.result;

    document.getElementById("issueDetails").innerHTML = `

        <h2>${issue.issueTitle}</h2>

        <p><b>Project :</b> ${issue.projectName}</p>

        <p><b>Status :</b> ${issue.status}</p>

        <p><b>Priority :</b> ${issue.priority}</p>

        <p><b>Description :</b> ${issue.description}</p>

        <p><b>Root Cause :</b> ${issue.rootCause}</p>

        <p><b>Solution :</b> ${issue.solution}</p>

    `;

}

loadIssue();