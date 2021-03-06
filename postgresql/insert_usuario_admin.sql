-- senha: admin
insert into dealwish.usuarios (email, senha, nome, cpf, aplicativo, retaguarda, empresa, id_situacao, data_nasc) 
values ('admin@dealwish.com.br', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 'Admin', '20277722004','N','S', 'N', 1, '1900-01-01');
/
insert into dealwish.grp_usr (id_usuario, id_grp_permissao) values ((select id from dealwish.usuarios where email = 'admin@dealwish.xyz'), (select id from dealwish.grp_permissoes where codigo = 'tin'));
/
commit;
/