import { api } from './api.js';

const STATES = ['Todo', 'InProgress', 'Done'];

const PRIORITY = {
  High:   { dots: '●●●', label: 'срочно' },
  Medium: { dots: '●●○', label: 'обычно' },
  Low:    { dots: '●○○', label: 'не горит' },
};

let tasks = [];
let search = '';
let lastMovedId = null;

const $ = (selector, root = document) => root.querySelector(selector);

// ---------- маленькие помощники ----------

// Создаёт элемент без innerHTML: текст задач вставляется безопасно.
function el(tag, props = {}, ...children) {
  const node = document.createElement(tag);
  for (const [key, value] of Object.entries(props)) {
    if (key === 'class') node.className = value;
    else if (key.startsWith('on')) node.addEventListener(key.slice(2), value);
    else if (value !== false && value != null) node.setAttribute(key, value === true ? '' : value);
  }
  node.append(...children);
  return node;
}

function todayISO() {
  return new Date().toLocaleDateString('sv-SE'); // формат гггг-мм-дд
}

function formatDate(iso) {
  const [year, month, day] = iso.split('-').map(Number);
  return new Date(year, month - 1, day).toLocaleDateString('ru-RU', { day: 'numeric', month: 'short' });
}

let toastTimer;
function toast(message) {
  const box = $('#toast');
  box.textContent = message;
  box.classList.add('show');
  clearTimeout(toastTimer);
  toastTimer = setTimeout(() => box.classList.remove('show'), 3500);
}

// ---------- отрисовка ----------

function actionButton(symbol, label, disabled, onClick) {
  return el('button', { type: 'button', 'aria-label': label, title: label, disabled, onclick: onClick }, symbol);
}

function renderCard(task) {
  const index = STATES.indexOf(task.state);
  const overdue = task.dueDate && task.state !== 'Done' && task.dueDate < todayISO();
  const priority = PRIORITY[task.priority];

  const meta = el('div', { class: 'meta' },
    el('span', { class: `prio prio-${task.priority.toLowerCase()}` },
      el('span', { 'aria-hidden': 'true' }, priority.dots), ' ', priority.label));

  if (task.dueDate) {
    meta.append(el('span', { class: overdue ? 'due overdue' : 'due' },
      (overdue ? 'просрочено, ' : 'до ') + formatDate(task.dueDate)));
  }

  const classes = ['card'];
  if (task.id === lastMovedId) classes.push('landed');

  const card = el('li', {
      class: classes.join(' '),
      draggable: 'true',
      'data-id': task.id,
      ondragstart: (event) => {
        event.dataTransfer.setData('text/plain', String(task.id));
        event.dataTransfer.effectAllowed = 'move';
        card.classList.add('dragging');
      },
      ondragend: () => card.classList.remove('dragging'),
    },
    el('p', { class: 'title' }, task.title),
    meta,
    el('div', { class: 'actions' },
      actionButton('←', 'Сдвинуть назад', index === 0, () => moveTask(task.id, STATES[index - 1])),
      actionButton('→', 'Сдвинуть вперёд', index === STATES.length - 1, () => moveTask(task.id, STATES[index + 1])),
      actionButton('×', 'Удалить дело', false, () => removeTask(task.id)),
    ));

  return card;
}

function renderEmpty(state) {
  const text = state === 'Done'
    ? 'Здесь появятся готовые дела.'
    : search
      ? 'Ничего не нашлось.'
      : 'Пусто. Добавьте дело выше или перетащите сюда карточку.';
  return el('li', { class: 'empty' }, text);
}

function renderBoard() {
  for (const column of document.querySelectorAll('.col')) {
    const state = column.dataset.state;
    const items = tasks.filter((task) => task.state === state);
    const list = $('.cards', column);

    list.replaceChildren(...(items.length ? items.map(renderCard) : [renderEmpty(state)]));
    $('[data-count]', column).textContent = items.length;
  }
  lastMovedId = null;
}

function renderSummary(stats) {
  const left = stats.todo + stats.inProgress;
  let text;

  if (stats.total === 0) text = 'Дел пока нет. Добавьте первое.';
  else if (left === 0) text = 'Всё сделано.';
  else {
    text = `Осталось дел: ${left}.`;
    if (stats.overdue > 0) text += ` Просрочено: ${stats.overdue}.`;
  }

  $('#summary').textContent = text;
}

// ---------- действия ----------

async function refresh() {
  try {
    const [list, stats] = await Promise.all([api.list(search), api.stats()]);
    tasks = list;
    renderBoard();
    renderSummary(stats);
  } catch (error) {
    $('#summary').textContent = 'Не получилось загрузить дела.';
    toast(error.message);
  }
}

async function moveTask(id, state) {
  const task = tasks.find((item) => item.id === id);
  if (!task || task.state === state) return;

  try {
    await api.move(id, state);
    lastMovedId = id;
    await refresh();
  } catch (error) {
    toast(error.message);
  }
}

async function removeTask(id) {
  try {
    await api.remove(id);
    await refresh();
    toast('Дело удалено.');
  } catch (error) {
    toast(error.message);
  }
}

// ---------- события ----------

const form = $('#add-form');

form.addEventListener('submit', async (event) => {
  event.preventDefault();
  const data = new FormData(form);

  try {
    await api.add(data.get('title'), data.get('priority'), data.get('dueDate'));
    form.reset();
    $('input[name="title"]', form).focus();
    await refresh();
  } catch (error) {
    toast(error.message);
  }
});

let searchTimer;
$('#search').addEventListener('input', (event) => {
  clearTimeout(searchTimer);
  searchTimer = setTimeout(() => {
    search = event.target.value;
    refresh();
  }, 200);
});

for (const column of document.querySelectorAll('.col')) {
  column.addEventListener('dragover', (event) => {
    event.preventDefault();
    column.classList.add('over');
  });

  column.addEventListener('dragleave', (event) => {
    if (!column.contains(event.relatedTarget)) column.classList.remove('over');
  });

  column.addEventListener('drop', (event) => {
    event.preventDefault();
    column.classList.remove('over');
    const id = Number(event.dataTransfer.getData('text/plain'));
    if (id) moveTask(id, column.dataset.state);
  });
}

refresh();
