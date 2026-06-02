using Learnix.data;
using Learnix.model;

namespace Learnix.control;

public class CadastroController
{
    public Aluno? CadastrarAluno(string nome, string email, string senha)
    {
        using var ctx = new LearnixDbContext();
        if (ctx.Usuarios.Any(u => u.Email == email)) return null;

        string matricula = email.Contains('@') ? email.Split('@')[0].ToUpper() : email.ToUpper();
        if (ctx.Alunos.Any(a => a.MatriculaAcademica == matricula)) return null;

        var perfil = new PerfilDeAprendizagem
        {
            EstiloPredominante = "Não definido",
            RitmoSugerido = "Regular",
        };
        ctx.PerfisDeAprendizagem.Add(perfil);
        ctx.SaveChanges();

        var aluno = new Aluno
        {
            Nome = nome,
            Email = email,
            Senha = senha,
            MatriculaAcademica = matricula,
            DataCadastro = DateTime.Now,
            PerfilDeAprendizagemId = perfil.Id,
        };
        ctx.Alunos.Add(aluno);
        ctx.SaveChanges();
        return aluno;
    }

    public Instrutor? CadastrarInstrutor(string nome, string email, string senha, string especialidade)
    {
        using var ctx = new LearnixDbContext();
        if (ctx.Usuarios.Any(u => u.Email == email)) return null;

        var instrutor = new Instrutor
        {
            Nome = nome,
            Email = email,
            Senha = senha,
            Especialidade = especialidade,
            Biografia = string.Empty,
            DataCadastro = DateTime.Now,
        };
        instrutor.Definir();
        ctx.Instrutores.Add(instrutor);
        ctx.SaveChanges();
        return instrutor;
    }
}