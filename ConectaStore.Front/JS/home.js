async function carregarCategorias() {
    const container = document.getElementById("categorias");
    try {
        var categorias = await getCategorias();
    } catch {
        container.innerHTML = '<p class="text-on-surface-variant col-span-full text-center py-12">API indisponível. Verifique se o servidor está rodando.</p>';
        return;
    }
    container.innerHTML = "";
    categorias.forEach(cat => {
        container.innerHTML += `
        <a href="produtos.html" class="block relative h-64 rounded-xl overflow-hidden group cursor-pointer border border-white/5" style="background: ${cat.cor || '#171f33'}">
            <div class="absolute inset-0 bg-gradient-to-t from-background via-transparent to-transparent"></div>
            <div class="absolute inset-0 flex items-center justify-center">
                <span class="material-symbols-outlined text-6xl opacity-20" style="color: ${cat.cor || '#38bdf8'}">category</span>
            </div>
            <div class="absolute bottom-4 left-4">
                <span class="text-xs uppercase tracking-widest text-primary font-bold">Categoria</span>
                <h4 class="text-xl font-bold text-on-surface">${cat.nome}</h4>
            </div>
        </a>`;
    });
}

async function carregarDestaques() {
    const container = document.getElementById("destaques");
    try {
        const response = await fetch(`${API}/produtos/destaques`);
        if (!response.ok) {
            container.innerHTML = '<p class="text-on-surface-variant col-span-full text-center py-12">Nenhum produto em destaque no momento.</p>';
            return;
        }
        var produtos = await response.json();
    } catch {
        if (!container.innerHTML.trim()) container.innerHTML = '<p class="text-on-surface-variant col-span-full text-center py-12">API indisponível. Verifique se o servidor está rodando.</p>';
        return;
    }
    container.innerHTML = "";
    if (!produtos.length) {
        container.innerHTML = '<p class="text-on-surface-variant col-span-full text-center py-12">Nenhum produto em destaque no momento.</p>';
        return;
    }
    produtos.forEach(p => {
        container.innerHTML += `
        <div class="glass-card rounded-2xl overflow-hidden flex flex-col">
            <div class="relative h-64 bg-surface-container-high group overflow-hidden">
                <img class="w-full h-full object-contain p-8 group-hover:scale-105 transition-transform duration-500" src="${p.foto || ''}" alt="${p.nome}" loading="lazy">
                <div class="absolute top-4 right-4 bg-primary text-on-primary font-label-sm text-label-sm px-3 py-1 rounded-full font-bold">Destaque</div>
            </div>
            <div class="p-6 flex-1 flex flex-col">
                <div class="flex justify-between items-start mb-2">
                    <span class="text-primary font-label-sm text-label-sm uppercase tracking-wide">${p.categoria?.nome || 'Geral'}</span>
                </div>
                <h3 class="font-headline-md text-headline-md text-on-surface mb-4">${p.nome}</h3>
                <p class="text-on-surface-variant text-sm mb-4 line-clamp-2">${p.descricao || ''}</p>
                <div class="mt-auto flex items-center justify-between">
                    <div>
                        <span class="text-on-surface-variant text-sm block">A partir de</span>
                        <span class="text-primary font-bold text-2xl">R$ ${Number(p.valorVenda).toFixed(2).replace('.', ',')}</span>
                    </div>
                    <a href="produto.html?id=${p.id}" class="bg-primary/10 hover:bg-primary text-primary hover:text-on-primary p-3 rounded-xl transition-all">
                        <span class="material-symbols-outlined">chevron_right</span>
                    </a>
                </div>
            </div>
        </div>`;
    });
}

carregarCategorias();
carregarDestaques();
