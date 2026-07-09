const BASE_URL = "http://localhost:5049";

async function getAllIssues() {
    const response = await fetch(BASE_URL + "/viewIssues");
    return await response.json();
}

async function getIssue(id) {
    const response = await fetch(BASE_URL + "/viewIssue/" + id);
    return await response.json();
}

async function addIssue(issue) {
    const response = await fetch(BASE_URL + "/addIssue", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(issue)
    });
    return await response.json();
}

async function updateIssue(id, issue) {
    const response = await fetch(BASE_URL + "/updateIssue/" + id, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(issue)
    });
    return await response.json();
}

async function deleteIssue(id) {
    const response = await fetch(BASE_URL + "/deleteIssue/" + id, {
        method: "DELETE"
    });
    return await response.json();
}
