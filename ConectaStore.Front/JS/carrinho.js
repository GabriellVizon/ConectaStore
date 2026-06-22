function adicionarAoCarrinho(produto) {
    let carrinho = JSON.parse(localStorage.getItem("carrinho") || "[]");
    const existente = carrinho.find(item => item.id === produto.id);
    if (existente) {
        if (existente.quantidade < produto.qtde) {
            existente.quantidade++;
        } else {
            mostrarToast("Estoque máximo atingido para este produto.", "warning");
            return;
        }
    } else {
        if (produto.qtde <= 0) {
            mostrarToast("Produto fora de estoque.", "error");
            return;
        }
        carrinho.push({
            id: produto.id,
            nome: produto.nome,
            foto: produto.foto,
            preco: produto.valorVenda,
            quantidade: 1,
            estoque: produto.qtde
        });
    }
    localStorage.setItem("carrinho", JSON.stringify(carrinho));
    atualizarBadgeCarrinho();
    mostrarToast(`${produto.nome} adicionado ao carrinho!`, "success");
}

function removerDoCarrinho(id) {
    let carrinho = JSON.parse(localStorage.getItem("carrinho") || "[]");
    const item = carrinho.find(i => i.id === id);
    carrinho = carrinho.filter(item => item.id !== id);
    localStorage.setItem("carrinho", JSON.stringify(carrinho));
    atualizarBadgeCarrinho();
    if (typeof renderizarCarrinho === "function") renderizarCarrinho();
}

function atualizarQuantidade(id, delta) {
    let carrinho = JSON.parse(localStorage.getItem("carrinho") || "[]");
    const item = carrinho.find(i => i.id === id);
    if (!item) return;
    const novaQtd = item.quantidade + delta;
    if (novaQtd <= 0) {
        carrinho = carrinho.filter(i => i.id !== id);
    } else if (novaQtd > item.estoque) {
        mostrarToast("Estoque insuficiente.", "warning");
        return;
    } else {
        item.quantidade = novaQtd;
    }
    localStorage.setItem("carrinho", JSON.stringify(carrinho));
    atualizarBadgeCarrinho();
    if (typeof renderizarCarrinho === "function") renderizarCarrinho();
}

function getCarrinho() {
    return JSON.parse(localStorage.getItem("carrinho") || "[]");
}

function getTotalCarrinho() {
    return getCarrinho().reduce((total, item) => total + item.preco * item.quantidade, 0);
}

function getContagemCarrinho() {
    return getCarrinho().reduce((total, item) => total + item.quantidade, 0);
}

function limparCarrinho() {
    localStorage.removeItem("carrinho");
    atualizarBadgeCarrinho();
    if (typeof renderizarCarrinho === "function") renderizarCarrinho();
}

function atualizarBadgeCarrinho() {
    document.querySelectorAll(".badge-carrinho").forEach(el => {
        const count = getContagemCarrinho();
        el.textContent = count;
        el.style.display = count > 0 ? "flex" : "none";
    });
}

function mostrarToast(mensagem, tipo) {
    const container = document.getElementById("toast-container");
    if (!container) return;
    const cores = { success: "bg-green-500", error: "bg-red-500", warning: "bg-yellow-500" };
    const toast = document.createElement("div");
    toast.className = `${cores[tipo] || 'bg-primary'} text-white px-6 py-3 rounded-xl shadow-lg font-label-md text-label-md animate-slide-up`;
    toast.textContent = mensagem;
    container.appendChild(toast);
    setTimeout(() => { toast.remove(); }, 3000);
}

document.addEventListener("DOMContentLoaded", atualizarBadgeCarrinho);
