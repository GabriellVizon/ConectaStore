const params = new URLSearchParams(window.location.search);
const id = params.get("id");

async function carregarProduto() {
    const container = document.getElementById("produto");
    if (!id) {
        container.innerHTML = '<p class="text-on-surface-variant text-center py-12">Nenhum produto selecionado.</p>';
        return;
    }
    let produto;
    try {
        const response = await fetch(`${API}/produtos/${id}`);
        if (!response.ok) {
            container.innerHTML = '<p class="text-on-surface-variant text-center py-12">Produto não encontrado.</p>';
            return;
        }
        produto = await response.json();
        window._produtoDetalhe = produto;
    } catch {
        container.innerHTML = '<p class="text-on-surface-variant text-center py-12">API indisponível. Verifique se o servidor está rodando.</p>';
        return;
    }
    container.innerHTML = `
    <div class="grid grid-cols-1 lg:grid-cols-12 gap-xl">
        <div class="lg:col-span-7">
            <div class="relative aspect-square glass-card rounded-xl overflow-hidden group">
                <img class="w-full h-full object-contain p-8 transform transition-transform duration-700 group-hover:scale-110" src="${produto.foto || ''}" alt="${produto.nome}">
                ${produto.destaque ? `<div class="absolute top-md right-md"><span class="px-md py-xs glass-card rounded-full font-label-sm text-label-sm text-primary uppercase tracking-widest">Destaque Tech</span></div>` : ''}
            </div>
        </div>
        <div class="lg:col-span-5 flex flex-col space-y-lg">
            <div>
                <div class="flex items-center gap-sm mb-xs">
                    <span class="text-primary font-label-md text-label-md uppercase tracking-[0.2em]">${produto.categoria?.nome || 'Geral'}</span>
                </div>
                <h1 class="font-headline-lg text-[42px] leading-[1.1] mb-md text-on-surface font-extrabold tracking-tight">${produto.nome}</h1>
                <div class="flex items-center gap-md">
                    <span class="px-md py-xs bg-green-500/10 text-green-400 border border-green-500/20 rounded-full font-label-sm text-label-sm flex items-center">
                        <span class="material-symbols-outlined text-[14px] mr-1" style="font-variation-settings: 'FILL' 1;">check_circle</span>
                        ${produto.qtde > 0 ? 'EM ESTOQUE' : 'FORA DE ESTOQUE'}
                    </span>
                </div>
            </div>
            <div class="p-lg glass-card rounded-xl premium-glow">
                <div class="mb-sm text-on-surface-variant font-label-md text-label-md">Preço para Cooperado</div>
                <div class="flex items-baseline gap-sm mb-lg">
                    <span class="text-primary font-bold text-[48px] tracking-tighter">R$ ${Number(produto.valorVenda).toFixed(2).replace('.', ',')}</span>
                </div>
                <div class="flex flex-col gap-md">
                    <button class="w-full py-md bg-gradient-to-r from-primary-container to-[#004c69] text-on-primary font-bold rounded-xl shadow-lg hover:shadow-primary/20 transition-all transform hover:-translate-y-1 active:scale-[0.98] flex justify-center items-center gap-md" onclick="adicionarAoCarrinho(window._produtoDetalhe)">
                        <span class="material-symbols-outlined">add_shopping_cart</span>
                        ADICIONAR AO CARRINHO
                    </button>
                </div>
            </div>

            <div class="flex gap-md">
                <div class="p-md glass-card rounded-lg flex items-center gap-md flex-1">
                    <span class="material-symbols-outlined text-primary">inventory_2</span>
                    <div>
                        <div class="text-[10px] text-on-surface-variant font-label-sm uppercase">Estoque</div>
                        <div class="text-body-md font-semibold">${produto.qtde} unidades</div>
                    </div>
                </div>
            </div>
            <a href="produtos.html" class="flex items-center text-on-surface-variant hover:text-primary transition-all group">
                <span class="material-symbols-outlined mr-2 group-hover:-translate-x-1 transition-transform">arrow_back</span>
                <span class="font-label-md text-label-md uppercase tracking-widest">Voltar ao Catálogo</span>
            </a>
        </div>
    </div>
    <div class="mt-xl grid grid-cols-1 lg:grid-cols-12 gap-xl">
        <div class="lg:col-span-8">
            <div class="border-b border-white/10 mb-lg">
                <div class="flex gap-xl overflow-x-auto">
                    <button class="pb-md border-b-2 border-primary text-primary font-bold whitespace-nowrap">Descrição Detalhada</button>
                </div>
            </div>
            <article class="prose prose-invert max-w-none text-on-surface-variant space-y-md">
                <p class="text-body-lg font-body-lg">${produto.descricao || 'Descrição não disponível.'}</p>
            </article>
        </div>
    </div>`;
}

carregarProduto();
