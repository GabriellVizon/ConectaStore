let listaProdutos = [];
let categorias = [];

async function carregarProdutos() {
    try {
        listaProdutos = await getProdutos();
    } catch {
        document.getElementById("product-grid").innerHTML = '<p class="text-on-surface-variant col-span-full text-center py-12">API indisponível. Verifique se o servidor está rodando.</p>';
        return;
    }
    const params = new URLSearchParams(window.location.search);
    const filtro = params.get("filter");
    let exibidos;
    if (filtro === "destaques") {
        exibidos = listaProdutos.filter(p => p.destaque);
    } else {
        exibidos = listaProdutos;
    }
    renderizarProdutos(exibidos);
    atualizarContagem(exibidos.length);
    destacarBotaoCategoria("todos");
}

async function carregarCategoriasSidebar() {
    try {
        categorias = await getCategorias();
    } catch { return; }
    const container = document.getElementById("categorias-sidebar");
    if (!container) return;
    let html = `<button class="w-full flex items-center justify-between p-3 rounded-xl bg-primary-container/20 text-primary border-r-4 border-primary text-label-md font-label-md group transition-all" data-categoria-id="todos" onclick="filtrarPorCategoria('todos')">
        <span>Todos os Produtos</span>
        <span class="bg-primary/20 px-2 py-0.5 rounded text-xs">${listaProdutos.length}</span>
    </button>`;
    categorias.forEach(cat => {
        const count = listaProdutos.filter(p => p.categoriaId === cat.id).length;
        html += `<button class="w-full flex items-center justify-between p-3 rounded-xl text-on-surface-variant hover:bg-white/5 text-label-md font-label-md transition-all" data-categoria-id="${cat.id}" onclick="filtrarPorCategoria(${cat.id})">
            <span>${cat.nome}</span>
            <span class="bg-white/5 px-2 py-0.5 rounded text-xs text-outline">${count}</span>
        </button>`;
    });
    container.innerHTML = html;
}

function filtrarPorCategoria(categoriaId) {
    const params = new URLSearchParams(window.location.search);
    const isOfertas = params.get("filter") === "destaques";
    let base = isOfertas ? listaProdutos.filter(p => p.destaque) : listaProdutos;
    if (categoriaId === "todos") {
        renderizarProdutos(base);
        atualizarContagem(base.length);
    } else {
        const filtrados = base.filter(p => p.categoriaId === categoriaId);
        renderizarProdutos(filtrados);
        atualizarContagem(filtrados.length);
    }
    destacarBotaoCategoria(categoriaId);
}

function destacarBotaoCategoria(id) {
    document.querySelectorAll("[data-categoria-id]").forEach(btn => {
        const bid = btn.getAttribute("data-categoria-id");
        if (String(bid) === String(id)) {
            btn.className = "w-full flex items-center justify-between p-3 rounded-xl bg-primary-container/20 text-primary border-r-4 border-primary text-label-md font-label-md group transition-all";
        } else {
            btn.className = "w-full flex items-center justify-between p-3 rounded-xl text-on-surface-variant hover:bg-white/5 text-label-md font-label-md transition-all";
        }
    });
}

window._produtosMap = {};

function renderizarProdutos(produtos) {
    const container = document.getElementById("product-grid");
    container.innerHTML = "";
    if (!produtos.length) {
        container.innerHTML = '<p class="text-on-surface-variant col-span-full text-center py-12">Nenhum produto encontrado.</p>';
        return;
    }
    window._produtosMap = {};
    produtos.forEach(p => {
        window._produtosMap[p.id] = p;
        const temDestaque = p.destaque;
        container.innerHTML += `
        <article class="group glass rounded-2xl overflow-hidden neon-border-hover transition-all duration-300">
            <div class="relative h-64 overflow-hidden bg-surface-container">
                <img class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110" src="${p.foto || ''}" alt="${p.nome}" loading="lazy">
                ${temDestaque ? '<span class="absolute top-4 left-4 bg-primary/20 backdrop-blur-md text-primary text-[10px] font-bold uppercase tracking-widest px-3 py-1 rounded-full border border-primary/30">Destaque</span>' : ''}
                <button class="absolute top-4 right-4 bg-surface/80 p-2 rounded-full text-on-surface-variant hover:text-error transition-colors">
                    <span class="material-symbols-outlined">favorite</span>
                </button>
            </div>
            <div class="p-lg">
                <div class="text-primary text-[10px] font-bold uppercase tracking-widest mb-1">${p.categoria?.nome || 'Geral'}</div>
                <h2 class="font-headline-md text-headline-md text-on-surface mb-2">${p.nome}</h2>
                <div class="flex items-center justify-between gap-md">
                    <span class="text-primary font-bold text-headline-md">R$ ${Number(p.valorVenda).toFixed(2).replace('.', ',')}</span>
                    <div class="flex items-center gap-sm">
                        <button class="bg-primary/10 hover:bg-primary text-primary hover:text-on-primary p-2.5 rounded-xl transition-all active:scale-95 add-cart-btn" data-pid="${p.id}">
                            <span class="material-symbols-outlined text-lg">add_shopping_cart</span>
                        </button>
                        <a href="produto.html?id=${p.id}" class="bg-surface-container-high hover:bg-white/10 text-on-surface-variant hover:text-primary p-2.5 rounded-xl transition-all active:scale-95">
                            <span class="material-symbols-outlined text-lg">chevron_right</span>
                        </a>
                    </div>
                </div>
            </div>
        </article>`;
    });
}

document.addEventListener("click", function (e) {
    const btn = e.target.closest(".add-cart-btn");
    if (btn) {
        const pid = parseInt(btn.getAttribute("data-pid"));
        const prod = window._produtosMap[pid];
        if (prod) adicionarAoCarrinho(prod);
    }
});

function atualizarContagem(mostrando) {
    const elMostrar = document.querySelector("#contagem-mostrar");
    const elTotal = document.querySelector("#contagem-total");
    if (elMostrar) elMostrar.textContent = mostrando;
    if (elTotal) elTotal.textContent = listaProdutos.length;
}

function ordenarProdutos(criterio) {
    let ordenados = [...listaProdutos];
    switch (criterio) {
        case 'menor-preco':
            ordenados.sort((a, b) => a.valorVenda - b.valorVenda);
            break;
        case 'maior-preco':
            ordenados.sort((a, b) => b.valorVenda - a.valorVenda);
            break;
        case 'nome':
            ordenados.sort((a, b) => a.nome.localeCompare(b.nome));
            break;
        default:
            ordenados = [...listaProdutos];
    }
    const params = new URLSearchParams(window.location.search);
    if (params.get("filter") === "destaques") {
        ordenados = ordenados.filter(p => p.destaque);
    }
    renderizarProdutos(ordenados);
    atualizarContagem(ordenados.length);
}

document.addEventListener("DOMContentLoaded", function () {
    const busca = document.getElementById("busca");
    if (busca) {
        busca.addEventListener("keyup", function () {
            const texto = this.value.toLowerCase();
            const filtrados = listaProdutos.filter(p =>
                p.nome.toLowerCase().includes(texto) ||
                (p.descricao && p.descricao.toLowerCase().includes(texto))
            );
            renderizarProdutos(filtrados);
            atualizarContagem(filtrados.length);
        });
    }
    const sortSelect = document.getElementById("sort-select");
    if (sortSelect) {
        sortSelect.addEventListener("change", function () {
            ordenarProdutos(this.value);
        });
    }
});

carregarProdutos().then(() => carregarCategoriasSidebar());
