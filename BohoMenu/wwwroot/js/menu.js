// فیلتر کردن آیتم‌ها در مرورگر و هایلایت تب فعال
const q = document.getElementById('q');
const empty = document.getElementById('empty');
const cats = [...document.querySelectorAll('.cat')];
const tabFor = cat => document.querySelector(`#tabs a[href="#${cat.id}"]`);

q?.addEventListener('input', () => {
  const term = q.value.trim().toLowerCase();
  let any = false;
  cats.forEach(cat => {
    let visible = 0;
    cat.querySelectorAll('.item').forEach(item => {
      const match = !term || item.textContent.toLowerCase().includes(term);
      item.hidden = !match;
      if (match) visible++;
    });
    cat.hidden = visible === 0;
    tabFor(cat).hidden = visible === 0;
    if (visible) any = true;
  });
  empty.hidden = any;
});

const observer = new IntersectionObserver(entries => {
  entries.forEach(e => {
    if (!e.isIntersecting) return;
    document.querySelectorAll('#tabs a').forEach(a => {
      const on = a.hash === '#' + e.target.id;
      a.classList.toggle('on', on);
      if (on) a.scrollIntoView({ inline: 'center', block: 'nearest' });
    });
  });
}, { rootMargin: '-30% 0px -60% 0px' });
cats.forEach(c => observer.observe(c));

// بعد از هر بار بارگذاری، تب فعال را وسط نوار دسته‌ها بیاور (برای موبایل)
document.querySelector('#tabs a.on')?.scrollIntoView({ inline: 'center', block: 'nearest' });
