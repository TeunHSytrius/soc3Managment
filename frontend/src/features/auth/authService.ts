export async function sendLoginRequest(
  endpoint: string,
  credentials: Record<string, string>,
): Promise<Response> {
  const response = await fetch(endpoint, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(credentials),
  });

  return response;
}
