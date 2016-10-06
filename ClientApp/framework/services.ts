export class ApiServices
{
    static Get<T>(route: string)
    {
        return fetch(route,
            {
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                method: "GET",
                credentials: 'same-origin'
            })
            .then(response => response.json())
            .then((data: T) => data);
    }

    static Post<T>(route: string, payload: any)
    {
        return fetch(route,
            {
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                method: "POST",
                credentials: 'same-origin',
                body: JSON.stringify(payload)
            })
            .then(response => response.json())
            .then((data: T) => data);
    }
}