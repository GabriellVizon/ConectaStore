const API = "http://localhost:5278/api";

async function getCategorias() {
    const response = await fetch(`${API}/categorias`);
    return await response.json();
}

async function getProdutos() {
    const response = await fetch(`${API}/produtos`);
    return await response.json();
}

async function getDestaques() {
    const response = await fetch(`${API}/produtos/destaques`);
    return await response.json();
}

async function getProduto(id) {
    const response = await fetch(`${API}/produtos/${id}`);
    return await response.json();
}