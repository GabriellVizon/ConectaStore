const AUTH_KEY = "conecta_token";
const USER_KEY = "conecta_user";

function getToken() {
    return localStorage.getItem(AUTH_KEY);
}

function getUser() {
    const data = localStorage.getItem(USER_KEY);
    return data ? JSON.parse(data) : null;
}

function isLoggedIn() {
    return !!getToken();
}

function salvarSessao(token, usuario) {
    localStorage.setItem(AUTH_KEY, token);
    localStorage.setItem(USER_KEY, JSON.stringify(usuario));
}

function logout() {
    localStorage.removeItem(AUTH_KEY);
    localStorage.removeItem(USER_KEY);
    atualizarNavbar();
    window.location.href = "index.html";
}

// === Login ===
async function entrar() {
    const email = document.getElementById("login-email").value.trim();
    const senha = document.getElementById("login-senha").value;
    const erroEl = document.getElementById("login-erro");

    if (!email || !senha) {
        erroEl.textContent = "Preencha todos os campos.";
        erroEl.classList.remove("hidden");
        return;
    }

    erroEl.classList.add("hidden");
    try {
        const res = await fetch(`${API}/auth/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email, senha })
        });
        const data = await res.json();
        if (!res.ok) {
            erroEl.textContent = data.mensagem || "Erro ao entrar.";
            erroEl.classList.remove("hidden");
            return;
        }
        salvarSessao(data.token, data.usuario);
        window.location.href = "index.html";
    } catch {
        erroEl.textContent = "Erro de conexão com o servidor.";
        erroEl.classList.remove("hidden");
    }
}

// === Cadastro ===
async function cadastrar() {
    const nome = document.getElementById("cad-nome").value.trim();
    const email = document.getElementById("cad-email").value.trim();
    const senha = document.getElementById("cad-senha").value;
    const confirmar = document.getElementById("cad-confirmar").value;
    const erroEl = document.getElementById("cad-erro");

    if (!nome || !email || !senha || !confirmar) {
        erroEl.textContent = "Preencha todos os campos.";
        erroEl.classList.remove("hidden");
        return;
    }
    if (senha.length < 6) {
        erroEl.textContent = "A senha deve ter pelo menos 6 caracteres.";
        erroEl.classList.remove("hidden");
        return;
    }
    if (senha !== confirmar) {
        erroEl.textContent = "As senhas não conferem.";
        erroEl.classList.remove("hidden");
        return;
    }

    erroEl.classList.add("hidden");
    try {
        const res = await fetch(`${API}/auth/register`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ nome, email, senha })
        });
        const data = await res.json();
        if (!res.ok) {
            erroEl.textContent = data.mensagem || "Erro ao cadastrar.";
            erroEl.classList.remove("hidden");
            return;
        }
        salvarSessao(data.token, data.usuario);
        window.location.href = "index.html";
    } catch {
        erroEl.textContent = "Erro de conexão com o servidor.";
        erroEl.classList.remove("hidden");
    }
}

// === Navbar ===
function atualizarNavbar() {
    document.querySelectorAll("#auth-nav").forEach(el => {
        if (isLoggedIn()) {
            const user = getUser();
            el.innerHTML = `
                <div class="relative group">
                    <button class="flex items-center gap-2 text-on-surface-variant hover:text-primary transition-all px-3 py-1.5 rounded-lg hover:bg-white/5">
                        <span class="material-symbols-outlined text-lg">account_circle</span>
                        <span class="text-sm font-medium hidden md:inline">${user.nome.split(" ")[0]}</span>
                    </button>
                    <div class="absolute right-0 mt-2 w-48 bg-surface-container-high border border-white/10 rounded-xl shadow-xl opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all duration-200 z-50">
                        <div class="p-3 border-b border-white/5">
                            <p class="text-sm font-medium text-on-surface">${user.nome}</p>
                            <p class="text-xs text-on-surface-variant truncate">${user.email}</p>
                        </div>
                        <button onclick="logout()" class="flex items-center gap-2 w-full p-3 text-sm text-on-surface-variant hover:text-error hover:bg-white/5 rounded-b-xl transition-all">
                            <span class="material-symbols-outlined text-lg">logout</span>
                            Sair
                        </button>
                    </div>
                </div>
            `;
        } else {
            el.innerHTML = `
                <a href="login.html" class="flex items-center gap-1 text-on-surface-variant hover:text-primary transition-all font-label-md text-label-md">
                    <span class="material-symbols-outlined text-lg">account_circle</span>
                    <span class="hidden md:inline">Entrar</span>
                </a>
            `;
        }
    });
}

document.addEventListener("DOMContentLoaded", atualizarNavbar);
