﻿namespace IntroAPI.Domain
{
    public class Aluno
    {
        public int Id { get; set; }
        public string RA { get; set; }  
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; } 
        
        public Cidade Cidade { get; set; }
    }
}
