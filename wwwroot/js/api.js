// Всё общение с сервером в одном месте.
// Остальной код не знает про fetch и адреса: он просто вызывает api.add(...), api.move(...).

async function request(url, options = {}) {
  let response;

  try {
    response = await fetch(url, {
      headers: { 'Content-Type': 'application/json' },
      ...options,
    });
  } catch {
    throw new Error('Нет связи с сервером. Проверьте, что запущен dotnet run.');
  }

  if (!response.ok) {
    let message = `Не получилось (код ${response.status}).`;
    try {
      const body = await response.json();
      if (body.error) message = body.error;
    } catch {
      // тело ответа не JSON, оставляем общее сообщение
    }
    throw new Error(message);
  }

  return response.status === 204 ? null : response.json();
}

export const api = {
  list: (search = '') =>
    request('/api/tasks' + (search ? `?search=${encodeURIComponent(search)}` : '')),

  stats: () => request('/api/tasks/stats'),

  add: (title, priority, dueDate) =>
    request('/api/tasks', {
      method: 'POST',
      body: JSON.stringify({ title, priority, dueDate: dueDate || null }),
    }),

  move: (id, state) =>
    request(`/api/tasks/${id}/state`, {
      method: 'PATCH',
      body: JSON.stringify({ state }),
    }),

  remove: (id) => request(`/api/tasks/${id}`, { method: 'DELETE' }),
};
