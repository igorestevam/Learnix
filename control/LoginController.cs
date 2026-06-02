using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix.control;

public class LoginController
{
    public Usuario? RealizarLogin(string codigoAcesso, string senha)
    {
        using var ctx = new LearnixDbContext();
        Aluno? aluno = ctx.Alunos
            .FirstOrDefault(a =>
                (a.MatriculaAcademica == codigoAcesso || a.Email == codigoAcesso)
                && a.Senha == senha);
        if (aluno != null) return aluno;

        return ctx.Instrutores
            .FirstOrDefault(i => i.Email == codigoAcesso && i.Senha == senha);
    }

    public string? RecuperarSenha(string email)
    {
        using var ctx = new LearnixDbContext();
        return ctx.Usuarios
            .Where(u => u.Email == email)
            .Select(u => u.Senha)
            .FirstOrDefault();
    }
}