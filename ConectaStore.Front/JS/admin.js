let _categorias = [];
let _produtos = [];
let _vendas = [];
let _editandoCategoriaId = null;
let _editandoProdutoId = null;

function abrirSecao(secao) {
    document.querySelectorAll(".admin-section").forEach(el => el.classList.add("hidden"));
    const alvo = document.getElementById(`section-${secao}`);
    if (alvo) alvo.classList.remove("hidden");
    document.querySelectorAll(".nav-admin a").forEach(el => {
        el.classList.remove("bg-primary-container/20", "text-primary", "border-r-4", "border-primary");
        el.classList.add("text-on-surface-variant");
    });
    const link = document.querySelector(`.nav-admin a[data-secao="${secao}"]`);
    if (link) {
        link.classList.remove("text-on-surface-variant");
        link.classList.add("bg-primary-container/20", "text-primary", "border-r-4", "border-primary");
    }
    document.getElementById("admin-titulo").textContent = secao.charAt(0).toUpperCase() + secao.slice(1);
    switch (secao) {
        case "dashboard": carregarDashboard(); break;
        case "categorias": carregarCategorias(); break;
        case "estoque": carregarEstoque(); break;
        case "vendas": carregarVendas(); break;
    }
}

// ===== Dashboard =====
async function carregarDashboard() {
    try {
        const [resP, resC, resV] = await Promise.all([
            fetch(`${API}/produtos/admin`),
            fetch(`${API}/categorias`),
            fetch(`${API}/vendas`)
        ]);
        const prods = await resP.json();
        const cats = await resC.json();
        const vends = await resV.json();
        _produtos = prods;
        document.getElementById("dash-total-produtos").textContent = prods.length;
        document.getElementById("dash-total-categorias").textContent = cats.length;
        document.getElementById("dash-total-vendas").textContent = vends.length;
        const baixo = prods.filter(p => p.qtde < 5).length;
        document.getElementById("dash-estoque-baixo").textContent = baixo;
        const receita = vends.reduce((s, v) => s + v.total, 0);
        document.getElementById("dash-receita").textContent = `R$ ${receita.toFixed(2).replace('.', ',')}`;
    } catch {
        document.querySelectorAll("#section-dashboard .text-4xl, #section-dashboard .text-5xl").forEach(el => el.textContent = "ERRO");
    }
}

// ===== Categorias =====
async function carregarCategorias() {
    const tbody = document.getElementById("tabela-categorias");
    try {
        const res = await fetch(`${API}/categorias`);
        _categorias = await res.json();
        if (!_categorias.length) {
            tbody.innerHTML = '<tr><td colspan="5" class="p-8 text-center text-on-surface-variant">Nenhuma categoria cadastrada.</td></tr>';
            return;
        }
        const prods = _produtos.length ? _produtos : await (await fetch(`${API}/produtos/admin`)).json();
        tbody.innerHTML = _categorias.map(c => {
            const count = prods.filter(p => p.categoriaId === c.id).length;
            return `<tr class="border-b border-white/5 hover:bg-white/5 transition-colors">
                <td class="p-4">${c.id}</td>
                <td class="p-4 font-semibold">${c.nome}</td>
                <td class="p-4"><span class="inline-block w-6 h-6 rounded-full border border-white/10" style="background:${c.cor}"></span> ${c.cor}</td>
                <td class="p-4">${count}</td>
                <td class="p-4 flex gap-2">
                    <button class="text-primary hover:bg-primary/10 p-2 rounded-lg transition-all" onclick="abrirModalCategoria(${c.id})" title="Editar"><span class="material-symbols-outlined text-lg">edit</span></button>
                    <button class="text-error hover:bg-error/10 p-2 rounded-lg transition-all" onclick="excluirCategoria(${c.id})" title="Excluir"><span class="material-symbols-outlined text-lg">delete</span></button>
                </td>
            </tr>`;
        }).join("");
    } catch {
        tbody.innerHTML = '<tr><td colspan="5" class="p-8 text-center text-error">Erro ao carregar categorias.</td></tr>';
    }
}

function abrirModalCategoria(id) {
    _editandoCategoriaId = id || null;
    const cat = id ? _categorias.find(c => c.id === id) : null;
    document.getElementById("modal-titulo").textContent = cat ? "Editar Categoria" : "Nova Categoria";
    document.getElementById("modal-corpo").innerHTML = `
        <div class="space-y-md">
            <div><label class="text-on-surface-variant text-sm block mb-1">Nome</label>
            <input id="form-cat-nome" class="w-full bg-surface-container border border-white/10 rounded-xl px-4 py-3 text-sm focus:outline-none focus:border-primary" value="${cat ? cat.nome : ''}" placeholder="Ex: Periféricos"/></div>
            <div><label class="text-on-surface-variant text-sm block mb-1">Cor</label>
            <div class="flex items-center gap-3">
                <input id="form-cat-cor" type="color" class="h-10 w-16 rounded-lg border border-white/10 bg-surface-container cursor-pointer" value="${cat ? cat.cor : '#38bdf8'}"/>
                <span class="text-on-surface-variant text-sm" id="cor-preview">${cat ? cat.cor : '#38bdf8'}</span>
            </div></div>
            <div class="flex gap-md pt-md">
                <button class="flex-1 bg-primary text-on-primary py-3 rounded-xl font-bold hover:scale-[1.02] transition-all" onclick="salvarCategoria()">${cat ? "Salvar" : "Criar"}</button>
                <button class="flex-1 bg-white/5 text-on-surface-variant py-3 rounded-xl font-bold hover:bg-white/10 transition-all" onclick="fecharModal()">Cancelar</button>
            </div>
        </div>`;
    document.getElementById("form-cat-cor").addEventListener("input", function() {
        document.getElementById("cor-preview").textContent = this.value;
    });
    document.getElementById("modal-overlay").classList.remove("hidden");
}

async function salvarCategoria() {
    const nome = document.getElementById("form-cat-nome").value.trim();
    const cor = document.getElementById("form-cat-cor").value;
    if (!nome) { alert("Nome é obrigatório."); return; }
    try {
        if (_editandoCategoriaId) {
            await fetch(`${API}/categorias/${_editandoCategoriaId}`, {
                method: "PUT", headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ id: _editandoCategoriaId, nome, cor })
            });
        } else {
            await fetch(`${API}/categorias`, {
                method: "POST", headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ nome, cor })
            });
        }
        fecharModal();
        carregarCategorias();
        carregarDashboard();
    } catch { alert("Erro ao salvar categoria."); }
}

async function excluirCategoria(id) {
    if (!confirm("Excluir esta categoria?")) return;
    try {
        const res = await fetch(`${API}/categorias/${id}`, { method: "DELETE" });
        if (!res.ok) {
            const msg = await res.text();
            alert(msg || "Não é possível excluir: categoria possui produtos vinculados.");
            return;
        }
        carregarCategorias();
        carregarDashboard();
    } catch { alert("Erro ao excluir."); }
}

// ===== Estoque =====
async function carregarEstoque() {
    const tbody = document.getElementById("tabela-estoque");
    try {
        const res = await fetch(`${API}/produtos/admin`);
        _produtos = await res.json();
        if (!_produtos.length) {
            tbody.innerHTML = '<tr><td colspan="8" class="p-8 text-center text-on-surface-variant">Nenhum produto cadastrado.</td></tr>';
            return;
        }
        renderizarTabelaEstoque(_produtos);
    } catch {
        tbody.innerHTML = '<tr><td colspan="8" class="p-8 text-center text-error">Erro ao carregar produtos.</td></tr>';
    }
}

function renderizarTabelaEstoque(prods) {
    const tbody = document.getElementById("tabela-estoque");
    if (!prods.length) {
        tbody.innerHTML = '<tr><td colspan="8" class="p-8 text-center text-on-surface-variant">Nenhum produto encontrado.</td></tr>';
        return;
    }
    tbody.innerHTML = prods.map(p => {
        const baixo = p.qtde < 5;
        return `<tr class="border-b border-white/5 hover:bg-white/5 transition-colors ${baixo ? 'bg-error/5' : ''}">
            <td class="p-4">${p.id}</td>
            <td class="p-4 font-semibold">${p.nome}</td>
            <td class="p-4 text-on-surface-variant">${p.categoriaNome || '-'}</td>
            <td class="p-4"><span class="${baixo ? 'text-error font-bold' : ''}">${p.qtde}</span>${baixo ? ' <span class="text-error text-xs">(baixo)</span>' : ''}</td>
            <td class="p-4">R$ ${Number(p.valorCusto).toFixed(2).replace('.', ',')}</td>
            <td class="p-4 text-primary font-semibold">R$ ${Number(p.valorVenda).toFixed(2).replace('.', ',')}</td>
            <td class="p-4">${p.destaque ? '<span class="text-primary">Sim</span>' : '<span class="text-on-surface-variant">Não</span>'}</td>
            <td class="p-4 flex gap-2">
                <button class="text-primary hover:bg-primary/10 p-2 rounded-lg transition-all" onclick="abrirModalProduto(${p.id})" title="Editar"><span class="material-symbols-outlined text-lg">edit</span></button>
                <button class="text-error hover:bg-error/10 p-2 rounded-lg transition-all" onclick="excluirProduto(${p.id})" title="Excluir"><span class="material-symbols-outlined text-lg">delete</span></button>
            </td>
        </tr>`;
    }).join("");
}

function filtrarEstoque() {
    const texto = document.getElementById("busca-estoque").value.toLowerCase();
    if (!texto) { renderizarTabelaEstoque(_produtos); return; }
    const filtrados = _produtos.filter(p =>
        p.nome.toLowerCase().includes(texto) ||
        (p.categoriaNome && p.categoriaNome.toLowerCase().includes(texto))
    );
    renderizarTabelaEstoque(filtrados);
}

async function abrirModalProduto(id) {
    _editandoProdutoId = id || null;
    let prod = null;
    if (id) prod = _produtos.find(p => p.id === id);
    if (!_categorias.length) {
        try {
            const res = await fetch(`${API}/categorias`);
            _categorias = await res.json();
        } catch { return; }
    }
    document.getElementById("modal-titulo").textContent = prod ? "Editar Produto" : "Novo Produto";
    const catOptions = _categorias.map(c =>
        `<option value="${c.id}" ${prod && prod.categoriaId === c.id ? 'selected' : ''}>${c.nome}</option>`
    ).join("");
    document.getElementById("modal-corpo").innerHTML = `
        <div class="space-y-md">
            <div><label class="text-on-surface-variant text-sm block mb-1">Nome</label>
            <input id="form-prod-nome" class="w-full bg-surface-container border border-white/10 rounded-xl px-4 py-3 text-sm focus:outline-none focus:border-primary" value="${prod ? prod.nome : ''}" placeholder="Nome do produto"/></div>
            <div><label class="text-on-surface-variant text-sm block mb-1">Categoria</label>
            <select id="form-prod-cat" class="w-full bg-surface-container border border-white/10 rounded-xl px-4 py-3 text-sm focus:outline-none focus:border-primary">${catOptions}</select></div>
            <div><label class="text-on-surface-variant text-sm block mb-1">Descrição</label>
            <textarea id="form-prod-desc" class="w-full bg-surface-container border border-white/10 rounded-xl px-4 py-3 text-sm focus:outline-none focus:border-primary" rows="3" placeholder="Descrição do produto...">${prod ? (prod.descricao || '') : ''}</textarea></div>
            <div class="grid grid-cols-3 gap-md">
                <div><label class="text-on-surface-variant text-sm block mb-1">Quantidade</label>
                <input id="form-prod-qtde" type="number" min="0" class="w-full bg-surface-container border border-white/10 rounded-xl px-4 py-3 text-sm focus:outline-none focus:border-primary" value="${prod ? prod.qtde : 0}"/></div>
                <div><label class="text-on-surface-variant text-sm block mb-1">Custo (R$)</label>
                <input id="form-prod-custo" type="number" step="0.01" min="0" class="w-full bg-surface-container border border-white/10 rounded-xl px-4 py-3 text-sm focus:outline-none focus:border-primary" value="${prod ? prod.valorCusto : ''}"/></div>
                <div><label class="text-on-surface-variant text-sm block mb-1">Venda (R$)</label>
                <input id="form-prod-venda" type="number" step="0.01" min="0" class="w-full bg-surface-container border border-white/10 rounded-xl px-4 py-3 text-sm focus:outline-none focus:border-primary" value="${prod ? prod.valorVenda : ''}"/></div>
            </div>
            <div><label class="text-on-surface-variant text-sm block mb-1">URL da Foto</label>
            <input id="form-prod-foto" class="w-full bg-surface-container border border-white/10 rounded-xl px-4 py-3 text-sm focus:outline-none focus:border-primary" value="${prod ? (prod.foto || '') : ''}" placeholder="https://..."/></div>
            <div class="flex items-center gap-3">
                <input id="form-prod-destaque" type="checkbox" class="w-5 h-5 rounded border-white/10 bg-surface-container" ${prod && prod.destaque ? 'checked' : ''}/>
                <label class="text-on-surface-variant text-sm" for="form-prod-destaque">Produto em Destaque</label>
            </div>
            <div class="flex gap-md pt-md">
                <button class="flex-1 bg-primary text-on-primary py-3 rounded-xl font-bold hover:scale-[1.02] transition-all" onclick="salvarProduto()">${prod ? "Salvar" : "Criar"}</button>
                <button class="flex-1 bg-white/5 text-on-surface-variant py-3 rounded-xl font-bold hover:bg-white/10 transition-all" onclick="fecharModal()">Cancelar</button>
            </div>
        </div>`;
    document.getElementById("modal-overlay").classList.remove("hidden");
}

async function salvarProduto() {
    const nome = document.getElementById("form-prod-nome").value.trim();
    const categoriaId = parseInt(document.getElementById("form-prod-cat").value);
    const qtde = parseInt(document.getElementById("form-prod-qtde").value) || 0;
    const valorCusto = parseFloat(document.getElementById("form-prod-custo").value) || 0;
    const valorVenda = parseFloat(document.getElementById("form-prod-venda").value) || 0;
    const descricao = document.getElementById("form-prod-desc").value.trim();
    const foto = document.getElementById("form-prod-foto").value.trim();
    const destaque = document.getElementById("form-prod-destaque").checked;
    if (!nome) { alert("Nome é obrigatório."); return; }
    const body = JSON.stringify({ nome, categoriaId, qtde, valorCusto, valorVenda, descricao, foto, destaque });
    try {
        if (_editandoProdutoId) {
            await fetch(`${API}/produtos/admin/${_editandoProdutoId}`, { method: "PUT", headers: { "Content-Type": "application/json" }, body });
        } else {
            await fetch(`${API}/produtos/admin`, { method: "POST", headers: { "Content-Type": "application/json" }, body });
        }
        fecharModal();
        carregarEstoque();
        carregarDashboard();
    } catch { alert("Erro ao salvar produto."); }
}

async function excluirProduto(id) {
    if (!confirm("Excluir este produto permanentemente?")) return;
    try {
        await fetch(`${API}/produtos/${id}`, { method: "DELETE" });
        carregarEstoque();
        carregarDashboard();
    } catch { alert("Erro ao excluir."); }
}

// ===== Vendas =====
async function carregarVendas() {
    const tbody = document.getElementById("tabela-vendas");
    try {
        const res = await fetch(`${API}/vendas`);
        _vendas = await res.json();
        if (!_vendas.length) {
            tbody.innerHTML = '<tr><td colspan="6" class="p-8 text-center text-on-surface-variant">Nenhuma venda realizada.</td></tr>';
            return;
        }
        tbody.innerHTML = _vendas.map(v => {
            const data = new Date(v.data);
            const dataStr = data.toLocaleDateString("pt-BR") + " " + data.toLocaleTimeString("pt-BR", { hour: "2-digit", minute: "2-digit" });
            const itens = v.itens || [];
            return `<tr class="border-b border-white/5 hover:bg-white/5 transition-all cursor-pointer" onclick="detalharVenda(${v.id})">
                <td class="p-4">${v.id}</td>
                <td class="p-4 font-semibold">${v.cliente}</td>
                <td class="p-4 text-on-surface-variant text-sm">${dataStr}</td>
                <td class="p-4">${itens.length} item(ns)</td>
                <td class="p-4 text-primary font-bold">R$ ${Number(v.total).toFixed(2).replace('.', ',')}</td>
                <td class="p-4"><span class="px-3 py-1 bg-green-500/10 text-green-400 border border-green-500/20 rounded-full text-xs font-bold">${v.status}</span></td>
            </tr>`;
        }).join("");
    } catch {
        tbody.innerHTML = '<tr><td colspan="6" class="p-8 text-center text-error">Erro ao carregar vendas.</td></tr>';
    }
}

function detalharVenda(id) {
    const venda = _vendas.find(v => v.id === id);
    if (!venda) return;
    const itens = venda.itens || [];
    const data = new Date(venda.data);
    document.getElementById("modal-titulo").textContent = `Venda #${venda.id}`;
    document.getElementById("modal-corpo").innerHTML = `
        <div class="space-y-md">
            <div class="grid grid-cols-2 gap-md text-sm">
                <div><span class="text-on-surface-variant">Cliente:</span> <span class="font-semibold">${venda.cliente}</span></div>
                <div><span class="text-on-surface-variant">Data:</span> <span class="font-semibold">${data.toLocaleDateString("pt-BR")} ${data.toLocaleTimeString("pt-BR", {hour:"2-digit",minute:"2-digit"})}</span></div>
                <div><span class="text-on-surface-variant">Status:</span> <span class="text-green-400 font-bold">${venda.status}</span></div>
                <div><span class="text-on-surface-variant">Total:</span> <span class="text-primary font-bold">R$ ${Number(venda.total).toFixed(2).replace('.', ',')}</span></div>
            </div>
            <div class="border-t border-white/10 pt-md">
                <h4 class="text-on-surface-variant text-sm uppercase tracking-widest mb-sm">Itens</h4>
                ${itens.length ? itens.map(i => `
                    <div class="flex justify-between items-center py-2 border-b border-white/5 text-sm">
                        <span>${i.produto?.nome || 'Produto #' + i.produtoId} <span class="text-on-surface-variant">x${i.quantidade}</span></span>
                        <span class="text-primary">R$ ${Number(i.precoUnitario * i.quantidade).toFixed(2).replace('.', ',')}</span>
                    </div>
                `).join("") : '<p class="text-on-surface-variant text-sm">Nenhum item registrado.</p>'}
            </div>
            <button class="w-full bg-white/5 text-on-surface-variant py-3 rounded-xl font-bold hover:bg-white/10 transition-all" onclick="fecharModal()">Fechar</button>
        </div>`;
    document.getElementById("modal-overlay").classList.remove("hidden");
}

// ===== Modal =====
function fecharModal() {
    document.getElementById("modal-overlay").classList.add("hidden");
    _editandoCategoriaId = null;
    _editandoProdutoId = null;
}

document.addEventListener("DOMContentLoaded", () => abrirSecao("dashboard"));
