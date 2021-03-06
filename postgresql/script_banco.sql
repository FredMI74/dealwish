CREATE TABLE dealwish.cidades (
    id     SERIAL NOT NULL,
    nome   VARCHAR(100) NOT NULL,
    uf     VARCHAR(2) NOT NULL
);

ALTER TABLE dealwish.cidades
    ADD CHECK ( uf IN (
        'AC',
        'AL',
        'AM',
        'AP',
        'BA',
        'CE',
        'DF',
        'ES',
        'GO',
        'MA',
        'MG',
        'MS',
        'MT',
        'PA',
        'PB',
        'PE',
        'PI',
        'PR',
        'RJ',
        'RN',
        'RO',
        'RR',
        'RS',
        'SC',
        'SE',
        'SP',
        'TO'
    ) );

COMMENT ON TABLE dealwish.cidades IS
    'Cadastro de cidades';

COMMENT ON COLUMN dealwish.cidades.id IS
    'identificação da cidade';

COMMENT ON COLUMN dealwish.cidades.nome IS
    'nome da cidade';

COMMENT ON COLUMN dealwish.cidades.uf IS
    'estado';

CREATE INDEX cidades_uf_idx ON
    dealwish.cidades (
        uf
    ASC );

CREATE UNIQUE INDEX cidades_nome_uf_idx ON
    dealwish.cidades (
        nome
    ASC,
        uf
    ASC );

CREATE UNIQUE INDEX cidades_id_idx ON
    dealwish.cidades (
        id
    ASC );

ALTER TABLE dealwish.cidades ADD CONSTRAINT cidades_pk PRIMARY KEY ( id );

CREATE TABLE dealwish.configuracoes (
    id               SERIAL NOT NULL,
    codigo           VARCHAR(50) NOT NULL,
    valor            VARCHAR(150) NOT NULL,
    id_usuario_log   BIGINT NOT NULL
);

COMMENT ON TABLE dealwish.configuracoes IS
    'Configurações do Sistema';

COMMENT ON COLUMN dealwish.configuracoes.id IS
    'identificação da configuração
';

COMMENT ON COLUMN dealwish.configuracoes.codigo IS
    'Código da configuração';

COMMENT ON COLUMN dealwish.configuracoes.valor IS
    'valor da configuração';

COMMENT ON COLUMN dealwish.configuracoes.id_usuario_log IS
    'usuário que incluiu ou alterou o registro';

CREATE UNIQUE INDEX configuracoes_cod_idx ON
    dealwish.configuracoes (
        codigo
    ASC );

CREATE UNIQUE INDEX configuracoes_valor_id_idx ON
    dealwish.configuracoes (
        valor
    ASC,
        id
    ASC );


CREATE INDEX configuracoes_id_usuario_log_idx ON
    dealwish.configuracoes (
        id_usuario_log
    ASC );

ALTER TABLE dealwish.configuracoes ADD CONSTRAINT configuracoes_pk PRIMARY KEY ( valor,
                                                                                 id );


CREATE TABLE dealwish.configuracoes_log
 (LOG_OPERATION CHAR(3) NOT NULL
 ,LOG_DATETIME TIMESTAMP NOT NULL
 ,id INTEGER NOT NULL
 ,codigo VARCHAR (50) NOT NULL
 ,valor VARCHAR (150) NOT NULL
 ,id_usuario_log BIGINT NOT NULL
 );


CREATE OR REPLACE function dealwish.proc_configuracoes_log_trg()  RETURNS trigger AS $$
 DECLARE 
  rec dealwish.configuracoes_log%ROWTYPE; 
  blank dealwish.configuracoes_log%ROWTYPE; 
  BEGIN 
    rec := blank; 
    IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN 
      rec.id := NEW.id; 
      rec.codigo := NEW.codigo; 
      rec.valor := NEW.valor; 
      rec.id_usuario_log := NEW.id_usuario_log; 
      rec.LOG_DATETIME := current_timestamp; 
      IF TG_OP = 'INSERT' THEN 
        rec.LOG_OPERATION := 'INS'; 
      ELSIF TG_OP = 'UPDATE' THEN 
        rec.LOG_OPERATION := 'UPD'; 
      END IF; 
    ELSIF TG_OP = 'DELETE' THEN 
      rec.id := OLD.id; 
      rec.codigo := OLD.codigo; 
      rec.valor := OLD.valor; 
      rec.id_usuario_log := OLD.id_usuario_log; 
      rec.LOG_DATETIME := current_timestamp; 
      rec.LOG_OPERATION := 'DEL'; 
    END IF; 
    INSERT into dealwish.configuracoes_log VALUES (rec.*); 
	RETURN NEW;
  END;
  $$ LANGUAGE plpgsql;
  
  
  CREATE TRIGGER configuracoes_log_trg
  AFTER INSERT OR 
  UPDATE OR 
  DELETE ON dealwish.configuracoes for each row 
  execute procedure dealwish.proc_configuracoes_log_trg();
  
  
  CREATE TABLE dealwish.contratos (
    id               SERIAL NOT NULL,
    id_empresa       INTEGER NOT NULL,
    id_plano         INTEGER NOT NULL,
    id_situacao      INTEGER NOT NULL,
    dia_vct          smallint NOT NULL,
    data_inicio      DATE NOT NULL,
    data_bloqueio    DATE,
    data_termino     DATE,
    id_usuario_log   BIGINT NOT NULL
);

ALTER TABLE dealwish.contratos ADD CHECK ( dia_vct BETWEEN 1 AND 20 );

COMMENT ON COLUMN dealwish.contratos.id IS
    'identificação do contrato';

COMMENT ON COLUMN dealwish.contratos.id_empresa IS
    'Identificação da empresa';

COMMENT ON COLUMN dealwish.contratos.id_plano IS
    'Identificação do plano escolhido pela empresa';

COMMENT ON COLUMN dealwish.contratos.id_situacao IS
    'identificação da situação';

COMMENT ON COLUMN dealwish.contratos.dia_vct IS
    'Dia de vencimento da fatura';

COMMENT ON COLUMN dealwish.contratos.data_inicio IS
    'Data de início do contrato';

COMMENT ON COLUMN dealwish.contratos.data_bloqueio IS
    'Data de bloqueio do contrato';

COMMENT ON COLUMN dealwish.contratos.data_termino IS
    'Data de término do contrato';

COMMENT ON COLUMN dealwish.contratos.id_usuario_log IS
    'usuário que incluiu ou alterou o registro';

CREATE UNIQUE INDEX contratos_id_idx ON
    dealwish.contratos (
        id
    ASC );

CREATE INDEX contratos_id_empresa_idx ON
    dealwish.contratos (
        id_empresa
    ASC );

CREATE INDEX contratos_id_plano_idx ON
    dealwish.contratos (
        id_plano
    ASC );

CREATE INDEX contratos_id_situacao_idx ON
    dealwish.contratos (
        id_situacao
    ASC );

CREATE INDEX contratos_id_usuario_log_idx ON
    dealwish.contratos (
        id_usuario_log
    ASC );

ALTER TABLE dealwish.contratos ADD CONSTRAINT contratos_pk PRIMARY KEY ( id );


CREATE TABLE dealwish.contratos_log
 (LOG_OPERATION CHAR(3) NOT NULL
 ,LOG_DATETIME DATE NOT NULL
 ,id INTEGER NOT NULL
 ,id_empresa INTEGER NOT NULL
 ,id_plano INTEGER NOT NULL
 ,id_situacao INTEGER NOT NULL
 ,dia_vct smallint NOT NULL
 ,data_inicio DATE NOT NULL
 ,data_bloqueio DATE
 ,data_termino DATE
 ,id_usuario_log BIGINT NOT NULL
 );
 

CREATE OR REPLACE function dealwish.proc_contratos_log_trg()  RETURNS trigger AS $$
 Declare 
  rec dealwish.contratos_log%ROWTYPE; 
  blank dealwish.contratos_log%ROWTYPE; 
  BEGIN 
    rec := blank; 
    IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN 
      rec.id := NEW.id; 
      rec.id_empresa := NEW.id_empresa; 
      rec.id_plano := NEW.id_plano; 
      rec.id_situacao := NEW.id_situacao; 
      rec.dia_vct := NEW.dia_vct; 
      rec.data_inicio := NEW.data_inicio; 
      rec.data_bloqueio := NEW.data_bloqueio; 
      rec.data_termino := NEW.data_termino; 
      rec.id_usuario_log := NEW.id_usuario_log; 
      rec.LOG_DATETIME := current_timestamp; 
      IF TG_OP = 'INSERT' THEN 
        rec.LOG_OPERATION := 'INS'; 
      ELSIF TG_OP = 'UPDATE' THEN 
        rec.LOG_OPERATION := 'UPD'; 
      END IF; 
    ELSIF TG_OP = 'DELETE' THEN 
      rec.id := OLD.id; 
      rec.id_empresa := OLD.id_empresa; 
      rec.id_plano := OLD.id_plano; 
      rec.id_situacao := OLD.id_situacao; 
      rec.dia_vct := OLD.dia_vct; 
      rec.data_inicio := OLD.data_inicio; 
      rec.data_bloqueio := OLD.data_bloqueio; 
      rec.data_termino := OLD.data_termino; 
      rec.id_usuario_log := OLD.id_usuario_log; 
      rec.LOG_DATETIME := current_timestamp; 
      rec.LOG_OPERATION := 'DEL'; 
    END IF; 
    INSERT into dealwish.contratos_log VALUES (rec.*); 
	RETURN NEW;
  END; 
  $$ LANGUAGE plpgsql;
  
  CREATE TRIGGER contratos_log_trg
  AFTER INSERT OR 
  UPDATE OR 
  DELETE ON dealwish.contratos for each row 
  execute procedure dealwish.proc_contratos_log_trg();
  
  CREATE TABLE dealwish.desejos (
    id                BIGSERIAL NOT NULL,
    descricao         VARCHAR(4000) NOT NULL,
    id_usuario        BIGINT NOT NULL,
    id_tipo_produto   INTEGER NOT NULL,
    id_situacao       INTEGER NOT NULL
);

COMMENT ON TABLE dealwish.desejos IS
    'Desejos do usuário de Aplicativo, o que ele deseja comprar';

COMMENT ON COLUMN dealwish.desejos.id IS
    'Identificação do produto desejado';

COMMENT ON COLUMN dealwish.desejos.descricao IS
    'Descrição do produto desejado';

COMMENT ON COLUMN dealwish.desejos.id_usuario IS
    'Identificação do usuário (usuarios.id)';

COMMENT ON COLUMN dealwish.desejos.id_tipo_produto IS
    'Identificação do tipo de produto desejado';

CREATE UNIQUE INDEX desejos_id_idx ON
    dealwish.desejos (
        id
    ASC );

CREATE INDEX desejos_id_usuario_idx ON
    dealwish.desejos (
        id_usuario
    ASC );

CREATE INDEX desejos_id_tipo_produto_idx ON
    dealwish.desejos (
        id_tipo_produto
    ASC );

CREATE INDEX desejos_id_situacao_idx ON
    dealwish.desejos (
        id_situacao
    ASC );

ALTER TABLE dealwish.desejos ADD CONSTRAINT desejos_pk PRIMARY KEY ( id );

CREATE TABLE dealwish.empresas (
    id                SERIAL NOT NULL,
    fantasia          VARCHAR(150) NOT NULL,
    razao_social      VARCHAR(150) NOT NULL,
    cnpj              VARCHAR(15) NOT NULL,
    insc_est          VARCHAR(30) NOT NULL,
    url               VARCHAR(150) NOT NULL,
    email_com         VARCHAR(150) NOT NULL,
    email_sac         VARCHAR(150) NOT NULL,
    fone_com          VARCHAR(15) NOT NULL,
    fone_sac          VARCHAR(15) NOT NULL,
    endereco          VARCHAR(150) NOT NULL,
    numero            VARCHAR(10) NOT NULL,
    complemento       VARCHAR(20),
    bairro            VARCHAR(30) NOT NULL,
    cep               VARCHAR(10) NOT NULL,
    endereco_cob      VARCHAR(150) NOT NULL,
    numero_cob        VARCHAR(10) NOT NULL,
    complemento_cob   VARCHAR(20),
    bairro_cob        VARCHAR(30) NOT NULL,
    cep_cob           VARCHAR(10) NOT NULL,
    id_cidade         INTEGER NOT NULL,
    id_cidade_cob     INTEGER NOT NULL,
    logo              text NOT NULL,
    id_qualificacao   INTEGER DEFAULT 0 NOT NULL,
    id_usuario_log    BIGINT NOT NULL
);



ALTER TABLE dealwish.empresas ADD CHECK  ( numero BETWEEN '0' AND '9' );

ALTER TABLE dealwish.empresas ADD CHECK ( cep BETWEEN '0' AND '9' );

ALTER TABLE dealwish.empresas ADD CHECK ( numero_cob BETWEEN '0' AND '9' );

ALTER TABLE dealwish.empresas ADD CHECK ( cep_cob BETWEEN '0' AND '9' );

COMMENT ON TABLE dealwish.empresas IS
    'Cadastro de empresas que podem realizar ofertas ao desejos dos usuários';

COMMENT ON COLUMN dealwish.empresas.id IS
    'Identificação da empresa';

COMMENT ON COLUMN dealwish.empresas.fantasia IS
    'Nome fantasia';

COMMENT ON COLUMN dealwish.empresas.razao_social IS
    'Razão social';

COMMENT ON COLUMN dealwish.empresas.cnpj IS
    'CNPJ';

COMMENT ON COLUMN dealwish.empresas.insc_est IS
    'Inscrição estadua';

COMMENT ON COLUMN dealwish.empresas.url IS
    'site da empresa';

COMMENT ON COLUMN dealwish.empresas.email_com IS
    'e-mail comercial  da empresa';

COMMENT ON COLUMN dealwish.empresas.email_sac IS
    'E-mail de atendimento ao consumidor';

COMMENT ON COLUMN dealwish.empresas.fone_com IS
    'Telefone comercial';

COMMENT ON COLUMN dealwish.empresas.fone_sac IS
    'Telefone de atendimento ao consumidor';

COMMENT ON COLUMN dealwish.empresas.endereco IS
    'endereço ';

COMMENT ON COLUMN dealwish.empresas.numero IS
    'númerodo endereço
';

COMMENT ON COLUMN dealwish.empresas.complemento IS
    'complemento de endereço';

COMMENT ON COLUMN dealwish.empresas.bairro IS
    'bairro';

COMMENT ON COLUMN dealwish.empresas.cep IS
    'CEP';

COMMENT ON COLUMN dealwish.empresas.endereco_cob IS
    'endereço de cobrança
';

COMMENT ON COLUMN dealwish.empresas.numero_cob IS
    'númerodo endereço de cobrança
';

COMMENT ON COLUMN dealwish.empresas.complemento_cob IS
    'complemento de endereço de cobrança';

COMMENT ON COLUMN dealwish.empresas.bairro_cob IS
    'bairro de cobrança';

COMMENT ON COLUMN dealwish.empresas.cep_cob IS
    'CEP de cobrança';

COMMENT ON COLUMN dealwish.empresas.id_cidade IS
    'cidade ';

COMMENT ON COLUMN dealwish.empresas.id_cidade_cob IS
    'cidade cobrança';

COMMENT ON COLUMN dealwish.empresas.logo IS
    'Ícone - PNG 64x64 - Base64 ';

COMMENT ON COLUMN dealwish.empresas.id_qualificacao IS
    'Identificação da qualificação';

COMMENT ON COLUMN dealwish.empresas.id_usuario_log IS
    'usuário que incluiu ou alterou o registro';

CREATE UNIQUE INDEX empresas_cnpj_idx ON
    dealwish.empresas (
        cnpj
    ASC );

CREATE UNIQUE INDEX empresas_id_idx ON
    dealwish.empresas (
        id
    ASC );

CREATE INDEX empresas_id_cidade_idx ON
    dealwish.empresas (
        id_cidade
    ASC );

CREATE INDEX empresas_id_cidade_cob_idx ON
    dealwish.empresas (
        id_cidade_cob
    ASC );

CREATE INDEX empresas_id_usuario_log_idx ON
    dealwish.empresas (
        id_usuario_log
    ASC );

CREATE INDEX empresas_id_qualificacao_idx ON
    dealwish.empresas (
        id_qualificacao
    ASC );

ALTER TABLE dealwish.empresas ADD CONSTRAINT empresas_pk PRIMARY KEY ( id );


CREATE TABLE dealwish.empresas_log
 (LOG_OPERATION CHAR(3) NOT NULL
 ,LOG_DATETIME DATE NOT NULL
 ,id INTEGER NOT NULL
 ,fantasia VARCHAR (150) NOT NULL
 ,razao_social VARCHAR (150) NOT NULL
 ,cnpj VARCHAR (15) NOT NULL
 ,insc_est VARCHAR (30) NOT NULL
 ,url VARCHAR (150) NOT NULL
 ,email_com VARCHAR (150) NOT NULL
 ,email_sac VARCHAR (150) NOT NULL
 ,fone_com VARCHAR (15) NOT NULL
 ,fone_sac VARCHAR (15) NOT NULL
 ,endereco VARCHAR (150) NOT NULL
 ,numero VARCHAR (10) NOT NULL
 ,complemento VARCHAR (20)
 ,bairro VARCHAR (30) NOT NULL
 ,cep VARCHAR (10) NOT NULL
 ,endereco_cob VARCHAR (150) NOT NULL
 ,numero_cob VARCHAR (10) NOT NULL
 ,complemento_cob VARCHAR (20)
 ,bairro_cob VARCHAR (30) NOT NULL
 ,cep_cob VARCHAR (10) NOT NULL
 ,id_cidade INTEGER NOT NULL
 ,id_cidade_cob INTEGER NOT NULL
 ,logo TEXT NOT NULL
 ,id_qualificacao INTEGER NOT NULL
 ,id_usuario_log BIGINT NOT NULL
 );

CREATE OR REPLACE function dealwish.proc_empresas_log_trg()  RETURNS trigger AS $$
 Declare 
  rec dealwish.empresas_log%ROWTYPE; 
  blank dealwish.empresas_log%ROWTYPE; 
  BEGIN 
    rec := blank; 
    IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN 
      rec.id := NEW.id; 
      rec.fantasia := NEW.fantasia; 
      rec.razao_social := NEW.razao_social; 
      rec.cnpj := NEW.cnpj; 
      rec.insc_est := NEW.insc_est; 
      rec.url := NEW.url; 
      rec.email_com := NEW.email_com; 
      rec.email_sac := NEW.email_sac; 
      rec.fone_com := NEW.fone_com; 
      rec.fone_sac := NEW.fone_sac; 
      rec.endereco := NEW.endereco; 
      rec.numero := NEW.numero; 
      rec.complemento := NEW.complemento; 
      rec.bairro := NEW.bairro; 
      rec.cep := NEW.cep; 
      rec.endereco_cob := NEW.endereco_cob; 
      rec.numero_cob := NEW.numero_cob; 
      rec.complemento_cob := NEW.complemento_cob; 
      rec.bairro_cob := NEW.bairro_cob; 
      rec.cep_cob := NEW.cep_cob; 
      rec.id_cidade := NEW.id_cidade; 
      rec.id_cidade_cob := NEW.id_cidade_cob; 
      rec.logo := NEW.logo; 
      rec.id_qualificacao := NEW.id_qualificacao; 
      rec.id_usuario_log := NEW.id_usuario_log; 
      rec.LOG_DATETIME := current_timestamp; 
      IF TG_OP = 'INSERT' THEN 
        rec.LOG_OPERATION := 'INS'; 
      ELSIF TG_OP = 'UPDATE' THEN 
        rec.LOG_OPERATION := 'UPD'; 
      END IF; 
    ELSIF TG_OP = 'DELETE' THEN 
      rec.id := OLD.id; 
      rec.fantasia := OLD.fantasia; 
      rec.razao_social := OLD.razao_social; 
      rec.cnpj := OLD.cnpj; 
      rec.insc_est := OLD.insc_est; 
      rec.url := OLD.url; 
      rec.email_com := OLD.email_com; 
      rec.email_sac := OLD.email_sac; 
      rec.fone_com := OLD.fone_com; 
      rec.fone_sac := OLD.fone_sac; 
      rec.endereco := OLD.endereco; 
      rec.numero := OLD.numero; 
      rec.complemento := OLD.complemento; 
      rec.bairro := OLD.bairro; 
      rec.cep := OLD.cep; 
      rec.endereco_cob := OLD.endereco_cob; 
      rec.numero_cob := OLD.numero_cob; 
      rec.complemento_cob := OLD.complemento_cob; 
      rec.bairro_cob := OLD.bairro_cob; 
      rec.cep_cob := OLD.cep_cob; 
      rec.id_cidade := OLD.id_cidade; 
      rec.id_cidade_cob := OLD.id_cidade_cob; 
      rec.logo := OLD.logo; 
      rec.id_qualificacao := OLD.id_qualificacao; 
      rec.id_usuario_log := OLD.id_usuario_log; 
      rec.LOG_DATETIME := current_timestamp; 
      rec.LOG_OPERATION := 'DEL'; 
    END IF; 
    INSERT into dealwish.empresas_log VALUES (rec.*); 
	RETURN NEW;
  END; 
  $$ LANGUAGE plpgsql;
  
  CREATE TRIGGER empresas_log_trg
  AFTER INSERT OR 
  UPDATE OR 
  DELETE ON dealwish.empresas for each row 
  execute procedure dealwish.proc_empresas_log_trg();
  
  CREATE TABLE dealwish.faturas (
    id               SERIAL NOT NULL,
    mes              smallint NOT NULL,
    ano              smallint NOT NULL,
    id_empresa       INTEGER NOT NULL,
    nosso_numero     VARCHAR(15) NOT NULL,
    valor            decimal(15,2) NOT NULL,
    data_vct         DATE NOT NULL,
    data_pg          DATE,
    multa            decimal(15,2),
    juros            decimal(15,2),
    valor_pg         decimal(15,2),
    qtd_ofertas      INTEGER DEFAULT 0 NOT NULL,
    id_situacao      INTEGER NOT NULL,
    num_nfe          INTEGER,
    serie_nfe        VARCHAR(5),
    id_usuario_log   BIGINT NOT NULL,
	pix              VARCHAR(1000)
);

ALTER TABLE dealwish.faturas ADD CHECK ( mes BETWEEN 1 AND 12 );

COMMENT ON TABLE dealwish.faturas IS
    'Faturas das empresas, controle de contas a pagar e emissão de notas';

COMMENT ON COLUMN dealwish.faturas.id IS
    'identificação da fatura';

COMMENT ON COLUMN dealwish.faturas.mes IS
    'mês de referência';

COMMENT ON COLUMN dealwish.faturas.ano IS
    'ano dde referência';

COMMENT ON COLUMN dealwish.faturas.id_empresa IS
    'Identificação da empresa (EMPRESAS)';

COMMENT ON COLUMN dealwish.faturas.nosso_numero IS
    'Nosso número do boleto';

COMMENT ON COLUMN dealwish.faturas.valor IS
    'valor da fatura';

COMMENT ON COLUMN dealwish.faturas.data_vct IS
    'data de vencimento';

COMMENT ON COLUMN dealwish.faturas.data_pg IS
    'data de pagamento';

COMMENT ON COLUMN dealwish.faturas.multa IS
    'valor da multa por pagamento em atraso';

COMMENT ON COLUMN dealwish.faturas.juros IS
    'valor do juros  por pagamento em atraso';

COMMENT ON COLUMN dealwish.faturas.valor_pg IS
    'valor pago';

COMMENT ON COLUMN dealwish.faturas.qtd_ofertas IS
    'Quantidades de ofertas realizadas no mês';

COMMENT ON COLUMN dealwish.faturas.id_situacao IS
    'Situação da fatura (SITUACOES)';

COMMENT ON COLUMN dealwish.faturas.num_nfe IS
    'Número da nota fiscal eletrônica';

COMMENT ON COLUMN dealwish.faturas.serie_nfe IS
    'Série da nota fiscal eletrônica';

COMMENT ON COLUMN dealwish.faturas.id_usuario_log IS
    'usuário que incluiu ou alterou o registro';
	
COMMENT ON COLUMN dealwish.faturas.pix IS
    'QRCode para pagamentos PIX';	

CREATE INDEX faturas_nossonum_idx ON
    dealwish.faturas (
        nosso_numero
    ASC );

CREATE INDEX faturas_mesano_idx ON
    dealwish.faturas (
        mes
    ASC,
        ano
    ASC );

CREATE INDEX faturas_nfe_idx ON
    dealwish.faturas (
        num_nfe
    ASC );

CREATE UNIQUE INDEX faturas_id_idx ON
    dealwish.faturas (
        id
    ASC );

CREATE INDEX faturas_id_empresa_idx ON
    dealwish.faturas (
        id_empresa
    ASC );

CREATE INDEX faturas_id_situacao_idx ON
    dealwish.faturas (
        id_situacao
    ASC );

CREATE INDEX faturas_id_usuario_log_idx ON
    dealwish.faturas (
        id_usuario_log
    ASC );

ALTER TABLE dealwish.faturas ADD CONSTRAINT faturas_pk PRIMARY KEY ( id );


CREATE TABLE dealwish.faturas_log
 (LOG_OPERATION CHAR(3) NOT NULL
 ,LOG_DATETIME DATE NOT NULL
 ,id INTEGER NOT NULL
 ,mes smallint NOT NULL
 ,ano smallint NOT NULL
 ,id_empresa INTEGER NOT NULL
 ,nosso_numero VARCHAR (15) NOT NULL
 ,valor decimal (15,2) NOT NULL
 ,data_vct DATE NOT NULL
 ,data_pg DATE
 ,multa decimal (15,2)
 ,juros decimal (15,2)
 ,valor_pg decimal (15,2)
 ,qtd_ofertas INTEGER NOT NULL
 ,id_situacao INTEGER NOT NULL
 ,num_nfe INTEGER
 ,serie_nfe VARCHAR (5)
 ,id_usuario_log BIGINT NOT NULL
 );

CREATE OR REPLACE function dealwish.proc_faturas_log_trg()  RETURNS trigger AS $$
 Declare 
  rec dealwish.faturas_log%ROWTYPE; 
  blank dealwish.faturas_log%ROWTYPE; 
  BEGIN 
    rec := blank; 
    IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN 
      rec.id := NEW.id; 
      rec.mes := NEW.mes; 
      rec.ano := NEW.ano; 
      rec.id_empresa := NEW.id_empresa; 
      rec.nosso_numero := NEW.nosso_numero; 
      rec.valor := NEW.valor; 
      rec.data_vct := NEW.data_vct; 
      rec.data_pg := NEW.data_pg; 
      rec.multa := NEW.multa; 
      rec.juros := NEW.juros; 
      rec.valor_pg := NEW.valor_pg; 
      rec.qtd_ofertas := NEW.qtd_ofertas; 
      rec.id_situacao := NEW.id_situacao; 
      rec.num_nfe := NEW.num_nfe; 
      rec.serie_nfe := NEW.serie_nfe; 
      rec.id_usuario_log := NEW.id_usuario_log; 
      rec.LOG_DATETIME := current_timestamp; 
      IF TG_OP = 'INSERT' THEN 
        rec.LOG_OPERATION := 'INS'; 
      ELSIF TG_OP = 'UPDATE' THEN 
        rec.LOG_OPERATION := 'UPD'; 
      END IF; 
    ELSIF TG_OP = 'DELETE' THEN 
      rec.id := OLD.id; 
      rec.mes := OLD.mes; 
      rec.ano := OLD.ano; 
      rec.id_empresa := OLD.id_empresa; 
      rec.nosso_numero := OLD.nosso_numero; 
      rec.valor := OLD.valor; 
      rec.data_vct := OLD.data_vct; 
      rec.data_pg := OLD.data_pg; 
      rec.multa := OLD.multa; 
      rec.juros := OLD.juros; 
      rec.valor_pg := OLD.valor_pg; 
      rec.qtd_ofertas := OLD.qtd_ofertas; 
      rec.id_situacao := OLD.id_situacao; 
      rec.num_nfe := OLD.num_nfe; 
      rec.serie_nfe := OLD.serie_nfe; 
      rec.id_usuario_log := OLD.id_usuario_log; 
      rec.LOG_DATETIME := current_timestamp; 
      rec.LOG_OPERATION := 'DEL'; 
    END IF; 
    INSERT into dealwish.faturas_log VALUES (rec.*); 
	RETURN NEW;
  END; 
  $$ LANGUAGE plpgsql;
  
  CREATE TRIGGER faturas_log_trg
  AFTER INSERT OR 
  UPDATE OR 
  DELETE ON dealwish.faturas for each row 
  execute procedure dealwish.proc_faturas_log_trg();
 
  
  CREATE TABLE dealwish.grp_permissoes (
    id          SERIAL NOT NULL,
    descricao   VARCHAR(50) NOT NULL,
    codigo      VARCHAR(3) NOT NULL
);

COMMENT ON TABLE dealwish.grp_permissoes IS
    'Grupo de permissões que podem ser atribuídas à um usuário';

COMMENT ON COLUMN dealwish.grp_permissoes.id IS
    'identificação do grupo de permissões';

COMMENT ON COLUMN dealwish.grp_permissoes.descricao IS
    'descrição do grupo';

COMMENT ON COLUMN dealwish.grp_permissoes.codigo IS
    'Código do grupo';

CREATE UNIQUE INDEX grp_permissoes_cod_idx ON
    dealwish.grp_permissoes (
        codigo
    ASC );

CREATE UNIQUE INDEX grp_permissoes_id_idx ON
    dealwish.grp_permissoes (
        id
    ASC );

ALTER TABLE dealwish.grp_permissoes ADD CONSTRAINT grupos_pk PRIMARY KEY ( id );

CREATE TABLE dealwish.grp_produtos (
    id               SERIAL NOT NULL,
    descricao        VARCHAR(50) NOT NULL,
    icone            TEXT NOT NULL,
    id_situacao      INTEGER DEFAULT 2 NOT NULL,
    ordem            smallint DEFAULT 9999 NOT NULL,
    id_usuario_log   BIGINT NOT NULL
);

COMMENT ON TABLE dealwish.grp_produtos IS
    'Gruipos de produtos (Eletrodomésticos, Informática, Vestuário, etc)';

COMMENT ON COLUMN dealwish.grp_produtos.id IS
    'Identificação do grupo de produtos';

COMMENT ON COLUMN dealwish.grp_produtos.descricao IS
    'Descrição do grupo de produtos';

COMMENT ON COLUMN dealwish.grp_produtos.icone IS
    'Ícone - PNG 64x64 - Base64 ';

COMMENT ON COLUMN dealwish.grp_produtos.id_situacao IS
    'Identificação da situação (SITUACOES)';

COMMENT ON COLUMN dealwish.grp_produtos.ordem IS
    'Ordem';

COMMENT ON COLUMN dealwish.grp_produtos.id_usuario_log IS
    'usuário que incluiu ou alterou o registro';

CREATE INDEX grp_produtos_pk_idx ON
    dealwish.grp_produtos (
        id
    ASC );

CREATE UNIQUE INDEX grp_produtos_descricao_idx ON
    dealwish.grp_produtos (
        descricao
    ASC );

CREATE INDEX grp_produtos_id_situacao_idx ON
    dealwish.grp_produtos (
        id_situacao
    ASC );


CREATE INDEX grp_produtos_id_usuario_log_idx ON
    dealwish.grp_produtos (
        id_usuario_log
    ASC );

ALTER TABLE dealwish.grp_produtos ADD CONSTRAINT grp_produtos_pk PRIMARY KEY ( id );

ALTER TABLE dealwish.grp_produtos ADD CONSTRAINT grp_produtos_desc_un UNIQUE ( descricao );


CREATE TABLE dealwish.grp_produtos_log
 (LOG_OPERATION CHAR(3) NOT NULL
 ,LOG_DATETIME DATE NOT NULL
 ,id INTEGER NOT NULL
 ,descricao VARCHAR (50) NOT NULL
 ,icone text NOT NULL
 ,id_situacao INTEGER NOT NULL
 ,ordem smallint NOT NULL
 ,id_usuario_log BIGINT NOT NULL
 );

CREATE OR REPLACE function dealwish.proc_grp_produtos_log_trg()  RETURNS trigger AS $$
 Declare 
  rec dealwish.grp_produtos_log%ROWTYPE; 
  blank dealwish.grp_produtos_log%ROWTYPE; 
  BEGIN 
    rec := blank; 
    IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN 
      rec.id := NEW.id; 
      rec.descricao := NEW.descricao; 
      rec.icone := NEW.icone; 
      rec.id_situacao := NEW.id_situacao; 
      rec.ordem := NEW.ordem; 
      rec.id_usuario_log := NEW.id_usuario_log; 
      rec.LOG_DATETIME := current_timestamp; 
      IF TG_OP = 'INSERT' THEN 
        rec.LOG_OPERATION := 'INS'; 
      ELSIF TG_OP = 'UPDATE' THEN 
        rec.LOG_OPERATION := 'UPD'; 
      END IF; 
    ELSIF TG_OP = 'DELETE' THEN 
      rec.id := OLD.id; 
      rec.descricao := OLD.descricao; 
      rec.icone := OLD.icone; 
      rec.id_situacao := OLD.id_situacao; 
      rec.ordem := OLD.ordem; 
      rec.id_usuario_log := OLD.id_usuario_log; 
      rec.LOG_DATETIME := current_timestamp; 
      rec.LOG_OPERATION := 'DEL'; 
    END IF; 
    INSERT into dealwish.grp_produtos_log VALUES (rec.*); 
	RETURN NEW;
  END; 
 $$ LANGUAGE plpgsql;
  
  CREATE TRIGGER grp_produtos_log_trg
  AFTER INSERT OR 
  UPDATE OR 
  DELETE ON dealwish.grp_produtos for each row 
  execute procedure dealwish.proc_grp_produtos_log_trg();  
  
  
  CREATE TABLE dealwish.grp_usr (
    id                 SERIAL NOT NULL,
    id_usuario         BIGINT NOT NULL,
    id_grp_permissao   INTEGER NOT NULL,
    id_usuario_log     BIGINT
);

COMMENT ON TABLE dealwish.grp_usr IS
    'Grupos de Usários a que o usuário pertence';

COMMENT ON COLUMN dealwish.grp_usr.id IS
    'identificação do grrupo do usuário';

COMMENT ON COLUMN dealwish.grp_usr.id_usuario_log IS
    'usuário que incluiu ou alterou o registro';

CREATE UNIQUE INDEX grp_usr_grp_idx ON
    dealwish.grp_usr (
        id_usuario
    ASC,
        id_grp_permissao
    ASC );

CREATE INDEX grp_usr_id_grp_permissao_idx ON
    dealwish.grp_usr (
        id_grp_permissao
    ASC );

CREATE INDEX grp_usr_id_usuario_log_idx ON
    dealwish.grp_usr (
        id_usuario_log
    ASC );
	
ALTER TABLE dealwish.grp_usr ADD CONSTRAINT grp_usr_pk PRIMARY KEY ( id );		

CREATE TABLE dealwish.grp_usr_log
 (LOG_OPERATION CHAR(3) NOT NULL
 ,LOG_DATETIME DATE NOT NULL
 ,id INTEGER
 ,id_usuario BIGINT NOT NULL
 ,id_grp_permissao INTEGER NOT NULL
 ,id_usuario_log INTEGER
 );

CREATE OR REPLACE function dealwish.proc_grp_usr_log_trg()  RETURNS trigger AS $$
 Declare 
  rec dealwish.grp_usr_log%ROWTYPE; 
  blank dealwish.grp_usr_log%ROWTYPE; 
  BEGIN 
    rec := blank; 
    IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN 
      rec.id := NEW.id; 
      rec.id_usuario := NEW.id_usuario; 
      rec.id_grp_permissao := NEW.id_grp_permissao; 
      rec.id_usuario_log := NEW.id_usuario_log; 
      rec.LOG_DATETIME := current_timestamp; 
      IF TG_OP = 'INSERT' THEN 
        rec.LOG_OPERATION := 'INS'; 
      ELSIF TG_OP = 'UPDATE' THEN 
        rec.LOG_OPERATION := 'UPD'; 
      END IF; 
    ELSIF TG_OP = 'DELETE' THEN 
      rec.id := OLD.id; 
      rec.id_usuario := OLD.id_usuario; 
      rec.id_grp_permissao := OLD.id_grp_permissao; 
      rec.id_usuario_log := OLD.id_usuario_log; 
      rec.LOG_DATETIME := current_timestamp; 
      rec.LOG_OPERATION := 'DEL'; 
    END IF; 
    INSERT into dealwish.grp_usr_log VALUES (rec.*); 
	RETURN NEW;
  END; 
 $$ LANGUAGE plpgsql;
 
   CREATE TRIGGER grp_usr_log_trg
  AFTER INSERT OR 
  UPDATE OR 
  DELETE ON dealwish.grp_usr for each row 
  execute procedure dealwish.proc_grp_usr_log_trg();  
  
  
  CREATE TABLE dealwish.ofertas (
    id               BIGSERIAL NOT NULL,
    id_desejo        BIGINT NOT NULL,
    id_empresa       INTEGER NOT NULL,
    valor            decimal(15,2) NOT NULL,
    url              VARCHAR(4000) NOT NULL,
    descricao        VARCHAR(4000) NOT NULL,
    validade         DATE NOT NULL,
    id_usuario       BIGINT NOT NULL,
    data             DATE DEFAULT current_date NOT NULL,
    id_situacao      INTEGER DEFAULT 1 NOT NULL,
    id_fatura        INTEGER,
    lida             CHAR(1) DEFAULT 'N' NOT NULL,
    like_unlike      CHAR(1) DEFAULT 'N' NOT NULL,
    destaque         CHAR(1) DEFAULT 'N' NOT NULL,
    id_usuario_log   BIGINT NOT NULL
);

ALTER TABLE dealwish.ofertas
    ADD CHECK ( lida IN (
        'N',
        'S'
    ) );

ALTER TABLE dealwish.ofertas
    ADD CHECK ( like_unlike IN (
        'L',
        'N',
        'U'
    ) );

ALTER TABLE dealwish.ofertas
    ADD CHECK ( destaque IN (
        'N',
        'S'
    ) );

COMMENT ON TABLE dealwish.ofertas IS
    'Registro das ofertas das empresas aos usuários';

COMMENT ON COLUMN dealwish.ofertas.url IS
    'LInk da oferta da empresa';

COMMENT ON COLUMN dealwish.ofertas.descricao IS
    'Descrição da oferta (marca, modelo, dimesões, etc.)';

COMMENT ON COLUMN dealwish.ofertas.validade IS
    'data de validade da oferta';

COMMENT ON COLUMN dealwish.ofertas.id_usuario IS
    'Identificação do usuário que incluiu a oferta';

COMMENT ON COLUMN dealwish.ofertas.data IS
    'Data da oferta';

COMMENT ON COLUMN dealwish.ofertas.id_situacao IS
    'Identificação da situação (SITUACOES)';

COMMENT ON COLUMN dealwish.ofertas.id_fatura IS
    'Identificação da fatura, nulo = oferta ainda não faturada';

COMMENT ON COLUMN dealwish.ofertas.lida IS
    'Oferta lida (S/N)';

COMMENT ON COLUMN dealwish.ofertas.like_unlike IS
    'Like Unlike da oferta (N/L/U)';

COMMENT ON COLUMN dealwish.ofertas.destaque IS
    'oferta destacada';

COMMENT ON COLUMN dealwish.ofertas.id_usuario_log IS
    'usuário que incluiu ou alterou o registro';

CREATE INDEX ofertas_data_idx ON
    dealwish.ofertas (
        data
    ASC );

CREATE INDEX ofertas_desejo_idx ON
    dealwish.ofertas (
        id_desejo
    ASC );

CREATE UNIQUE INDEX ofertas_id_idx ON
    dealwish.ofertas (
        id
    ASC );

CREATE INDEX ofertas_id_empresa_idx ON
    dealwish.ofertas (
        id_empresa
    ASC );

CREATE INDEX ofertas_id_usuario_idx ON
    dealwish.ofertas (
        id_usuario
    ASC );

CREATE INDEX ofertas_id_situacao_idx ON
    dealwish.ofertas (
        id_situacao
    ASC );

CREATE INDEX ofertas_id_fatura_idx ON
    dealwish.ofertas (
        id_fatura
    ASC );

CREATE INDEX ofertas_id_usuario_log_idx ON
    dealwish.ofertas (
        id_usuario_log
    ASC );

ALTER TABLE dealwish.ofertas ADD CONSTRAINT ofertas_pk PRIMARY KEY ( id );


CREATE TABLE dealwish.ofertas_log
 (LOG_OPERATION CHAR(3) NOT NULL
 ,LOG_DATETIME DATE NOT NULL
 ,id BIGINT NOT NULL
 ,id_desejo BIGINT NOT NULL
 ,id_empresa INTEGER NOT NULL
 ,valor decimal (15,2) NOT NULL
 ,url VARCHAR (4000) NOT NULL
 ,descricao VARCHAR (4000) NOT NULL
 ,validade DATE NOT NULL
 ,id_usuario BIGINT NOT NULL
 ,data DATE NOT NULL
 ,id_situacao INTEGER NOT NULL
 ,id_fatura INTEGER
 ,lida CHAR (1) NOT NULL
 ,like_unlike CHAR (1) NOT NULL
 ,destaque CHAR (1) NOT NULL
 ,id_usuario_log BIGINT NOT NULL
 );

CREATE OR REPLACE function dealwish.proc_ofertas_log_trg()  RETURNS trigger AS $$
 Declare 
  rec dealwish.ofertas_log%ROWTYPE; 
  blank dealwish.ofertas_log%ROWTYPE; 
  BEGIN 
    rec := blank; 
    IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN 
      rec.id := NEW.id; 
      rec.id_desejo := NEW.id_desejo; 
      rec.id_empresa := NEW.id_empresa; 
      rec.valor := NEW.valor; 
      rec.url := NEW.url; 
      rec.descricao := NEW.descricao; 
      rec.validade := NEW.validade; 
      rec.id_usuario := NEW.id_usuario; 
      rec.data := NEW.data; 
      rec.id_situacao := NEW.id_situacao; 
      rec.id_fatura := NEW.id_fatura; 
      rec.lida := NEW.lida; 
      rec.like_unlike := NEW.like_unlike; 
      rec.destaque := NEW.destaque; 
      rec.id_usuario_log := NEW.id_usuario_log; 
      rec.LOG_DATETIME := current_timestamp; 
      IF TG_OP = 'INSERT' THEN 
        rec.LOG_OPERATION := 'INS'; 
      ELSIF TG_OP = 'UPDATE' THEN 
        rec.LOG_OPERATION := 'UPD'; 
      END IF; 
    ELSIF TG_OP = 'DELETE' THEN 
      rec.id := OLD.id; 
      rec.id_desejo := OLD.id_desejo; 
      rec.id_empresa := OLD.id_empresa; 
      rec.valor := OLD.valor; 
      rec.url := OLD.url; 
      rec.descricao := OLD.descricao; 
      rec.validade := OLD.validade; 
      rec.id_usuario := OLD.id_usuario; 
      rec.data := OLD.data; 
      rec.id_situacao := OLD.id_situacao; 
      rec.id_fatura := OLD.id_fatura; 
      rec.lida := OLD.lida; 
      rec.like_unlike := OLD.like_unlike; 
      rec.destaque := OLD.destaque; 
      rec.id_usuario_log := OLD.id_usuario_log; 
      rec.LOG_DATETIME := current_timestamp; 
      rec.LOG_OPERATION := 'DEL'; 
    END IF; 
    INSERT into dealwish.ofertas_log VALUES (rec.*); 
	RETURN NEW;
  END; 
  $$ LANGUAGE plpgsql;
  

   CREATE TRIGGER ofertas_log_trg
  AFTER INSERT OR 
  UPDATE OR 
  DELETE ON dealwish.ofertas for each row 
  execute procedure dealwish.proc_ofertas_log_trg();  
  
  
  
  
  CREATE TABLE dealwish.perm_grp (
    id                 SERIAL NOT NULL,
    id_grp_permissao   INTEGER NOT NULL,
    id_permissao       INTEGER NOT NULL
);

COMMENT ON TABLE dealwish.perm_grp IS
    'Relacionamento N para M de Grp_Permissoes com Permissões';

COMMENT ON COLUMN dealwish.perm_grp.id IS
    'identificação do grupo permissões e permissões';

CREATE UNIQUE INDEX perm_grp_id_idx ON
    dealwish.perm_grp (
        id
    ASC );

CREATE INDEX perm_grp_id_grp_permissao_idx ON
    dealwish.perm_grp (
        id_grp_permissao
    ASC );

CREATE INDEX perm_grp_id_permissao_idx ON
    dealwish.perm_grp (
        id_permissao
    ASC );

ALTER TABLE dealwish.perm_grp ADD CONSTRAINT grp_perm_pk PRIMARY KEY ( id );

CREATE TABLE dealwish.permissoes (
    id          SERIAL NOT NULL,
    descricao   VARCHAR(100) NOT NULL,
    codigo      VARCHAR(150) NOT NULL
);

COMMENT ON TABLE dealwish.permissoes IS
    'Permissões dos usuários (menus, funcões, relatórios,  botões, etc.)';

COMMENT ON COLUMN dealwish.permissoes.id IS
    'identificação da permissão';

COMMENT ON COLUMN dealwish.permissoes.descricao IS
    'descrição da descrição
';

COMMENT ON COLUMN dealwish.permissoes.codigo IS
    'código da permissão';

CREATE UNIQUE INDEX permissoes_cod_idx ON
    dealwish.permissoes (
        codigo
    ASC );

CREATE UNIQUE INDEX permissoes_id_idx ON
    dealwish.permissoes (
        id
    ASC );

ALTER TABLE dealwish.permissoes ADD CONSTRAINT permissoes_pk PRIMARY KEY ( id );

CREATE TABLE dealwish.planos (
    id               SERIAL NOT NULL,
    descricao        VARCHAR(50) NOT NULL,
    qtd_ofertas      INTEGER NOT NULL,
    valor_mensal     decimal(15,2) NOT NULL,
    valor_oferta     decimal(15,2) NOT NULL,
    visualizacao     CHAR(1) NOT NULL,
    id_usuario_log   BIGINT NOT NULL
);

ALTER TABLE dealwish.planos
    ADD CHECK ( visualizacao IN (
        'E',
        'N',
        'P'
    ) );

COMMENT ON TABLE dealwish.planos IS
    'Planos das empresas';

COMMENT ON COLUMN dealwish.planos.id IS
    'identificação do Plano';

COMMENT ON COLUMN dealwish.planos.descricao IS
    'Descrição do Plano (Ouro, Prata e Bronze)';

COMMENT ON COLUMN dealwish.planos.qtd_ofertas IS
    'Número de ofertas mensais incluídas no plano';

COMMENT ON COLUMN dealwish.planos.valor_mensal IS
    'Valor mensal do plano';

COMMENT ON COLUMN dealwish.planos.valor_oferta IS
    'Valor para cada oferta avulsa após o término das ofertas contidas no plano';

COMMENT ON COLUMN dealwish.planos.visualizacao IS
    'visualizacao de ofertas de outras empresas (N/P/E)';

CREATE UNIQUE INDEX planos_desc_idx ON
    dealwish.planos (
        descricao
    ASC );

CREATE UNIQUE INDEX planos_id_idx ON
    dealwish.planos (
        id
    ASC );

CREATE INDEX planos_id_usuario_log_idx ON
    dealwish.planos (
        id_usuario_log
    ASC );

ALTER TABLE dealwish.planos ADD CONSTRAINT planos_pk PRIMARY KEY ( id );


CREATE TABLE dealwish.planos_log
 (LOG_OPERATION CHAR(3) NOT NULL
 ,LOG_DATETIME DATE NOT NULL
 ,id INTEGER NOT NULL
 ,descricao VARCHAR (50) NOT NULL
 ,qtd_ofertas INTEGER NOT NULL
 ,valor_mensal decimal (15,2) NOT NULL
 ,valor_oferta decimal (15,2) NOT NULL
 ,visualizacao CHAR (1) NOT NULL
 ,id_usuario_log BIGINT NOT NULL
 );

CREATE OR REPLACE function dealwish.proc_planos_log_trg()  RETURNS trigger AS $$
 Declare 
  rec dealwish.planos_log%ROWTYPE; 
  blank dealwish.planos_log%ROWTYPE; 
  BEGIN 
    rec := blank; 
    IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN 
      rec.id := NEW.id; 
      rec.descricao := NEW.descricao; 
      rec.qtd_ofertas := NEW.qtd_ofertas; 
      rec.valor_mensal := NEW.valor_mensal; 
      rec.valor_oferta := NEW.valor_oferta; 
      rec.visualizacao := NEW.visualizacao; 
      rec.id_usuario_log := NEW.id_usuario_log; 
      rec.LOG_DATETIME := current_timestamp; 
      IF TG_OP = 'INSERT' THEN 
        rec.LOG_OPERATION := 'INS'; 
      ELSIF TG_OP = 'UPDATE' THEN 
        rec.LOG_OPERATION := 'UPD'; 
      END IF; 
    ELSIF TG_OP = 'DELETE' THEN 
      rec.id := OLD.id; 
      rec.descricao := OLD.descricao; 
      rec.qtd_ofertas := OLD.qtd_ofertas; 
      rec.valor_mensal := OLD.valor_mensal; 
      rec.valor_oferta := OLD.valor_oferta; 
      rec.visualizacao := OLD.visualizacao; 
      rec.id_usuario_log := OLD.id_usuario_log; 
      rec.LOG_DATETIME := current_timestamp; 
      rec.LOG_OPERATION := 'DEL'; 
    END IF; 
    INSERT into dealwish.planos_log VALUES (rec.*); 
	RETURN NEW;
  END; 
  $$ LANGUAGE plpgsql;
  
     CREATE TRIGGER planos_log_trg
  AFTER INSERT OR 
  UPDATE OR 
  DELETE ON dealwish.planos for each row 
  execute procedure dealwish.proc_planos_log_trg(); 
  
  
  CREATE TABLE dealwish.qualificacoes (
    id          SERIAL NOT NULL,
    descricao   VARCHAR(100) NOT NULL
);

COMMENT ON TABLE dealwish.qualificacoes IS
    'Qualificação da empresa (informativo)';

COMMENT ON COLUMN dealwish.qualificacoes.id IS
    'identificação da qualificação';

COMMENT ON COLUMN dealwish.qualificacoes.descricao IS
    'descrição da qualificação
';

CREATE UNIQUE INDEX qualificacoes_id_idx ON
    dealwish.qualificacoes (
        id
    ASC );

ALTER TABLE dealwish.qualificacoes ADD CONSTRAINT qualificacoes_pk PRIMARY KEY ( id );

CREATE TABLE dealwish.situacoes (
    id          SERIAL NOT NULL,
    descricao   VARCHAR(50) NOT NULL,
    contratos   CHAR(1) DEFAULT 'N' NOT NULL,
    usuarios    CHAR(1) DEFAULT 'N' NOT NULL,
    desejos     CHAR(1) DEFAULT 'N' NOT NULL,
    faturas     CHAR(1) DEFAULT 'N' NOT NULL,
    ofertas     CHAR(1) DEFAULT 'N' NOT NULL,
    produtos    CHAR(1) DEFAULT 'N' NOT NULL
);

ALTER TABLE dealwish.situacoes
    ADD CHECK ( contratos IN (
        'N',
        'S'
    ) );

ALTER TABLE dealwish.situacoes
    ADD CHECK ( usuarios IN (
        'N',
        'S'
    ) );

ALTER TABLE dealwish.situacoes
    ADD CHECK ( desejos IN (
        'N',
        'S'
    ) );

ALTER TABLE dealwish.situacoes
    ADD CHECK ( faturas IN (
        'N',
        'S'
    ) );

ALTER TABLE dealwish.situacoes
    ADD CHECK ( ofertas IN (
        'N',
        'S'
    ) );

ALTER TABLE dealwish.situacoes
    ADD CHECK ( produtos IN (
        'N',
        'S'
    ) );

COMMENT ON TABLE dealwish.situacoes IS
    'Sistuação dos contratos';

COMMENT ON COLUMN dealwish.situacoes.id IS
    'idenitifcação da situação';

COMMENT ON COLUMN dealwish.situacoes.descricao IS
    'descrição da situação (Ativo, Bloqueado, Encerrado)';

COMMENT ON COLUMN dealwish.situacoes.contratos IS
    'situação aplica-se à contratos (S/N)';

COMMENT ON COLUMN dealwish.situacoes.usuarios IS
    'situação aplica-se à usuários (S/N)';

COMMENT ON COLUMN dealwish.situacoes.desejos IS
    'situação aplica-se à desejos (S/N)';

COMMENT ON COLUMN dealwish.situacoes.faturas IS
    'situação aplica-se à faturas (S/N)';

COMMENT ON COLUMN dealwish.situacoes.ofertas IS
    'situação aplica-se à ofertas  (S/N)';

COMMENT ON COLUMN dealwish.situacoes.produtos IS
    'situação aplica-se à grupos e tipos de produtos  (S/N)';

CREATE UNIQUE INDEX situacoes_id_idx ON
    dealwish.situacoes (
        id
    ASC );

ALTER TABLE dealwish.situacoes ADD CONSTRAINT situacao_pk PRIMARY KEY ( id );

CREATE TABLE dealwish.termo_servico (
    id         SERIAL NOT NULL,
    corrente   CHAR(1) NOT NULL,
    texto      text NOT NULL
);

ALTER TABLE dealwish.termo_servico
    ADD CHECK ( corrente IN (
        'N',
        'S'
    ) );

COMMENT ON COLUMN dealwish.termo_servico.id IS
    'identificação do termo de serviço';

COMMENT ON COLUMN dealwish.termo_servico.corrente IS
    'termo corrente (S/N)';

COMMENT ON COLUMN dealwish.termo_servico.texto IS
    'Texto do termo de serviço';

CREATE UNIQUE INDEX termo_servico_id_idx ON
    dealwish.termo_servico (
        id
    ASC );

ALTER TABLE dealwish.termo_servico ADD CONSTRAINT termo_servico_pk PRIMARY KEY ( id );

CREATE TABLE dealwish.tokens (
    id_usuario         BIGINT NOT NULL,
    token              VARCHAR(4000),
    val_token          DATE,
    token_app          VARCHAR(4000),
    num_falhas_login  smallint DEFAULT 0 NOT NULL
);

COMMENT ON COLUMN dealwish.tokens.id_usuario IS
    'Identificação do Usário';

COMMENT ON COLUMN dealwish.tokens.token IS
    'Token';

COMMENT ON COLUMN dealwish.tokens.val_token IS
    'validade do token';

COMMENT ON COLUMN dealwish.tokens.token_app IS
    'Token app para cloud messaging';

COMMENT ON COLUMN dealwish.tokens.num_falhas_login IS
    'Número de tentativas de login sem sucesso';

CREATE INDEX usuarios_token_idx ON
    dealwish.tokens (
        token
    ASC );

CREATE UNIQUE INDEX tokens_id_usuario_idx ON
    dealwish.tokens (
        id_usuario
    ASC );

ALTER TABLE dealwish.tokens ADD CONSTRAINT tokens_pk PRIMARY KEY ( id_usuario );

CREATE TABLE dealwish.tp_produtos (
    id               SERIAL NOT NULL,
    descricao        VARCHAR(50) NOT NULL,
    id_grp_prod      INTEGER NOT NULL,
    icone            text NOT NULL,
    preenchimento    VARCHAR(4000) DEFAULT 'Informe a marca, modelo, tipo, cor e outras características importantes.' NOT NULL,
    id_situacao      INTEGER DEFAULT 2 NOT NULL,
    ordem            smallint DEFAULT 9999 NOT NULL,
    id_usuario_log   BIGINT NOT NULL
);

COMMENT ON TABLE dealwish.tp_produtos IS
    'Tipos de produtos (TV, Notebook, Geladeira, Tênis, etc)';

COMMENT ON COLUMN dealwish.tp_produtos.id IS
    'Identificação do tipo de produto';

COMMENT ON COLUMN dealwish.tp_produtos.descricao IS
    'Descrição do tipo de produto';

COMMENT ON COLUMN dealwish.tp_produtos.id_grp_prod IS
    'ID do grupo de produto (grp_produtos.id)';

COMMENT ON COLUMN dealwish.tp_produtos.icone IS
    'Ícone - PNG 64x64 - Base64 ';

COMMENT ON COLUMN dealwish.tp_produtos.preenchimento IS
    'Dica de preenchimento';

COMMENT ON COLUMN dealwish.tp_produtos.id_situacao IS
    'Identificação da situação (SITUACOES)';

COMMENT ON COLUMN dealwish.tp_produtos.ordem IS
    'Ordem';

COMMENT ON COLUMN dealwish.tp_produtos.id_usuario_log IS
    'usuário que incluiu ou alterou o registro';

CREATE INDEX tp_produtos_tp_produtos_id_idx ON
    dealwish.tp_produtos (
        id
    ASC );

CREATE UNIQUE INDEX tp_produtos_descricao_idx ON
    dealwish.tp_produtos (
        descricao
    ASC );

CREATE INDEX tp_produtos_id_grp_prod_idx ON
    dealwish.tp_produtos (
        id_grp_prod
    ASC );

CREATE INDEX tp_produtos_id_situacao_idx ON
    dealwish.tp_produtos (
        id_situacao
    ASC );

CREATE INDEX tp_produtos_id_usuario_log_idx ON
    dealwish.tp_produtos (
        id_usuario_log
    ASC );

ALTER TABLE dealwish.tp_produtos ADD CONSTRAINT tp_produtos_pk PRIMARY KEY ( id );

ALTER TABLE dealwish.tp_produtos ADD CONSTRAINT tp_produtos_descricao_un UNIQUE ( descricao );


CREATE TABLE dealwish.tp_produtos_log
 (LOG_OPERATION CHAR(3) NOT NULL
 ,LOG_DATETIME DATE NOT NULL
 ,id INTEGER NOT NULL
 ,descricao VARCHAR (50) NOT NULL
 ,id_grp_prod INTEGER NOT NULL
 ,icone text NOT NULL
 ,preenchimento VARCHAR (4000) NOT NULL
 ,id_situacao INTEGER NOT NULL
 ,ordem smallint NOT NULL
 ,id_usuario_log BIGINT NOT NULL
 );

CREATE OR REPLACE function dealwish.proc_tp_produtos_log_trg()  RETURNS trigger AS $$
 Declare 
  rec dealwish.tp_produtos_log%ROWTYPE; 
  blank dealwish.tp_produtos_log%ROWTYPE; 
  BEGIN 
    rec := blank; 
    IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN 
      rec.id := NEW.id; 
      rec.descricao := NEW.descricao; 
      rec.id_grp_prod := NEW.id_grp_prod; 
      rec.icone := NEW.icone; 
      rec.preenchimento := NEW.preenchimento; 
      rec.id_situacao := NEW.id_situacao; 
      rec.ordem := NEW.ordem; 
      rec.id_usuario_log := NEW.id_usuario_log; 
      rec.LOG_DATETIME := current_timestamp; 
      IF TG_OP = 'INSERT' THEN 
        rec.LOG_OPERATION := 'INS'; 
      ELSIF TG_OP = 'UPDATE' THEN 
        rec.LOG_OPERATION := 'UPD'; 
      END IF; 
    ELSIF TG_OP = 'DELETE' THEN 
      rec.id := OLD.id; 
      rec.descricao := OLD.descricao; 
      rec.id_grp_prod := OLD.id_grp_prod; 
      rec.icone := OLD.icone; 
      rec.preenchimento := OLD.preenchimento; 
      rec.id_situacao := OLD.id_situacao; 
      rec.ordem := OLD.ordem; 
      rec.id_usuario_log := OLD.id_usuario_log; 
      rec.LOG_DATETIME := current_timestamp; 
      rec.LOG_OPERATION := 'DEL'; 
    END IF; 
    INSERT into dealwish.tp_produtos_log VALUES (rec.*); 
	RETURN NEW;
  END; 
  $$ LANGUAGE plpgsql;
  
       CREATE TRIGGER tp_produtos_log_trg
  AFTER INSERT OR 
  UPDATE OR 
  DELETE ON dealwish.tp_produtos for each row 
  execute procedure dealwish.proc_tp_produtos_log_trg(); 
  
  
  CREATE TABLE dealwish.usuarios (
    id               BIGSERIAL NOT NULL,
    email            VARCHAR(100) NOT NULL,
    senha            VARCHAR(4000) NOT NULL,
    nome             VARCHAR(100) NOT NULL,
	data_nasc        DATE NOT NULL,
    cpf              VARCHAR(15) NOT NULL,
    aplicativo       CHAR(1) DEFAULT 'N' NOT NULL,
    retaguarda       CHAR(1) DEFAULT  'N' NOT NULL,
    empresa          CHAR(1) DEFAULT  'N' NOT NULL,
    id_cidade_ap     INTEGER,
    id_situacao      INTEGER NOT NULL,
    id_empresa       INTEGER,
    id_usuario_log   BIGINT
);

ALTER TABLE dealwish.usuarios ADD CHECK ( cpf BETWEEN '0' AND '9' );

ALTER TABLE dealwish.usuarios
    ADD CONSTRAINT usuarios_aplicativo_ck CHECK ( aplicativo IN (
        'N',
        'S'
    ) );

ALTER TABLE dealwish.usuarios
    ADD CONSTRAINT usuarios_retaguarda_ck CHECK ( retaguarda IN (
        'N',
        'S'
    ) );

ALTER TABLE dealwish.usuarios
    ADD CONSTRAINT usuarios_loja_ck CHECK ( empresa IN (
        'N',
        'S'
    ) );

COMMENT ON TABLE dealwish.usuarios IS
    'Usuários, cadastro unificado onde o usuário pode ser do aplicativo, da empresa ou da retaguarda';

COMMENT ON COLUMN dealwish.usuarios.id IS
    'identificação do usuário';

COMMENT ON COLUMN dealwish.usuarios.email IS
    'e-mail do usuário, utilizado como nome do login';

COMMENT ON COLUMN dealwish.usuarios.senha IS
    'senha do usuário em MD5';

COMMENT ON COLUMN dealwish.usuarios.nome IS
    'nome do usuário';

COMMENT ON COLUMN dealwish.usuarios.aplicativo IS
    'Usuário do aplicativo (S/N)';

COMMENT ON COLUMN dealwish.usuarios.retaguarda IS
    'usuário do sistema (S/N)';

COMMENT ON COLUMN dealwish.usuarios.empresa IS
    'usuário de empresa  (S/N)';

COMMENT ON COLUMN dealwish.usuarios.id_cidade_ap IS
    'obrigatório quando usuário tipo aplicativo';

COMMENT ON COLUMN dealwish.usuarios.id_empresa IS
    'Identificação da empresa';

COMMENT ON COLUMN dealwish.usuarios.id_usuario_log IS
    'usuário que incluiu ou alterou o registro';

CREATE INDEX usuarios_id_idx ON
    dealwish.usuarios (
        id
    ASC );

CREATE INDEX usuarios_email_idx ON
    dealwish.usuarios (
        email
    ASC );

CREATE UNIQUE INDEX usuarios_cpf_idx ON
    dealwish.usuarios (
        cpf
    ASC );

CREATE INDEX usuarios_id_cidade_ap_idx ON
    dealwish.usuarios (
        id_cidade_ap
    ASC );

CREATE INDEX usuarios_id_situacao_idx ON
    dealwish.usuarios (
        id_situacao
    ASC );

CREATE INDEX usuarios_id_empresa_idx ON
    dealwish.usuarios (
        id_empresa
    ASC );

CREATE INDEX usuarios_id_usuario_log_idx ON
    dealwish.usuarios (
        id_usuario_log
    ASC );

ALTER TABLE dealwish.usuarios ADD CONSTRAINT usuarios_pk PRIMARY KEY ( id );

ALTER TABLE dealwish.usuarios ADD CONSTRAINT usuarios_email_un UNIQUE ( email );


CREATE TABLE dealwish.usuarios_log
 (LOG_OPERATION CHAR(3) NOT NULL
 ,LOG_DATETIME DATE NOT NULL
 ,id BIGINT NOT NULL
 ,email VARCHAR (100) NOT NULL
 ,senha VARCHAR (4000) NOT NULL
 ,nome VARCHAR (100) NOT NULL
 ,cpf VARCHAR (15) NOT NULL
 ,aplicativo CHAR (1) NOT NULL
 ,retaguarda CHAR (1) NOT NULL
 ,empresa CHAR (1) NOT NULL
 ,id_cidade_ap INTEGER
 ,id_situacao INTEGER NOT NULL
 ,id_empresa INTEGER
 ,id_usuario_log BIGINT
 );

CREATE OR REPLACE function dealwish.proc_usuarios_log_trg()  RETURNS trigger AS $$
 Declare 
  rec dealwish.usuarios_log%ROWTYPE; 
  blank dealwish.usuarios_log%ROWTYPE; 
  BEGIN 
    rec := blank; 
    IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN 
      rec.id := NEW.id; 
      rec.email := NEW.email; 
      rec.senha := NEW.senha; 
      rec.nome := NEW.nome; 
      rec.cpf := NEW.cpf; 
      rec.aplicativo := NEW.aplicativo; 
      rec.retaguarda := NEW.retaguarda; 
      rec.empresa := NEW.empresa; 
      rec.id_cidade_ap := NEW.id_cidade_ap; 
      rec.id_situacao := NEW.id_situacao; 
      rec.id_empresa := NEW.id_empresa; 
      rec.id_usuario_log := NEW.id_usuario_log; 
      rec.LOG_DATETIME := current_timestamp; 
      IF TG_OP = 'INSERT' THEN 
        rec.LOG_OPERATION := 'INS'; 
      ELSIF TG_OP = 'UPDATE' THEN 
        rec.LOG_OPERATION := 'UPD'; 
      END IF; 
    ELSIF TG_OP = 'DELETE' THEN 
      rec.id := OLD.id; 
      rec.email := OLD.email; 
      rec.senha := OLD.senha; 
      rec.nome := OLD.nome; 
      rec.cpf := OLD.cpf; 
      rec.aplicativo := OLD.aplicativo; 
      rec.retaguarda := OLD.retaguarda; 
      rec.empresa := OLD.empresa; 
      rec.id_cidade_ap := OLD.id_cidade_ap; 
      rec.id_situacao := OLD.id_situacao; 
      rec.id_empresa := OLD.id_empresa; 
      rec.id_usuario_log := OLD.id_usuario_log; 
      rec.LOG_DATETIME := current_timestamp; 
      rec.LOG_OPERATION := 'DEL'; 
    END IF; 
    INSERT into dealwish.usuarios_log VALUES (rec.*);
    RETURN NEW;	
  END; 
  $$ LANGUAGE plpgsql;
  
         CREATE TRIGGER usuarios_log_trg
  AFTER INSERT OR 
  UPDATE OR 
  DELETE ON dealwish.usuarios for each row 
  execute procedure dealwish.proc_usuarios_log_trg(); 
  
  
  
  ALTER TABLE dealwish.configuracoes
    ADD CONSTRAINT configuracoes_usuarios_fk FOREIGN KEY ( id_usuario_log )
        REFERENCES dealwish.usuarios ( id );

ALTER TABLE dealwish.contratos
    ADD CONSTRAINT contratos_empresas_fk FOREIGN KEY ( id_empresa )
        REFERENCES dealwish.empresas ( id );

ALTER TABLE dealwish.contratos
    ADD CONSTRAINT contratos_planos_fk FOREIGN KEY ( id_plano )
        REFERENCES dealwish.planos ( id );

ALTER TABLE dealwish.contratos
    ADD CONSTRAINT contratos_situacao_fk FOREIGN KEY ( id_situacao )
        REFERENCES dealwish.situacoes ( id );

ALTER TABLE dealwish.contratos
    ADD CONSTRAINT contratos_usuarios_fk FOREIGN KEY ( id_usuario_log )
        REFERENCES dealwish.usuarios ( id );

ALTER TABLE dealwish.desejos
    ADD CONSTRAINT desejos_situacoes_fk FOREIGN KEY ( id_situacao )
        REFERENCES dealwish.situacoes ( id );

ALTER TABLE dealwish.desejos
    ADD CONSTRAINT desejos_tp_produtos_fk FOREIGN KEY ( id_tipo_produto )
        REFERENCES dealwish.tp_produtos ( id );

ALTER TABLE dealwish.desejos
    ADD CONSTRAINT desejos_usuarios_fk FOREIGN KEY ( id_usuario )
        REFERENCES dealwish.usuarios ( id );

ALTER TABLE dealwish.empresas
    ADD CONSTRAINT empresas_cidades_fk FOREIGN KEY ( id_cidade )
        REFERENCES dealwish.cidades ( id );

ALTER TABLE dealwish.empresas
    ADD CONSTRAINT empresas_cidades_fkv2 FOREIGN KEY ( id_cidade_cob )
        REFERENCES dealwish.cidades ( id );

ALTER TABLE dealwish.empresas
    ADD CONSTRAINT empresas_qualificacoes_fk FOREIGN KEY ( id_qualificacao )
        REFERENCES dealwish.qualificacoes ( id );

ALTER TABLE dealwish.empresas
    ADD CONSTRAINT empresas_usuarios_fk FOREIGN KEY ( id_usuario_log )
        REFERENCES dealwish.usuarios ( id );

ALTER TABLE dealwish.faturas
    ADD CONSTRAINT faturas_empresas_fk FOREIGN KEY ( id_empresa )
        REFERENCES dealwish.empresas ( id );

ALTER TABLE dealwish.faturas
    ADD CONSTRAINT faturas_situacoes_fk FOREIGN KEY ( id_situacao )
        REFERENCES dealwish.situacoes ( id );

ALTER TABLE dealwish.faturas
    ADD CONSTRAINT faturas_usuarios_fk FOREIGN KEY ( id_usuario_log )
        REFERENCES dealwish.usuarios ( id );

ALTER TABLE dealwish.perm_grp
    ADD CONSTRAINT grp_perm_grp_permissoes_fk FOREIGN KEY ( id_grp_permissao )
        REFERENCES dealwish.grp_permissoes ( id );

ALTER TABLE dealwish.perm_grp
    ADD CONSTRAINT grp_perm_permissoes_fk FOREIGN KEY ( id_permissao )
        REFERENCES dealwish.permissoes ( id );

ALTER TABLE dealwish.grp_produtos
    ADD CONSTRAINT grp_produtos_situacoes_fk FOREIGN KEY ( id_situacao )
        REFERENCES dealwish.situacoes ( id );

ALTER TABLE dealwish.grp_produtos
    ADD CONSTRAINT grp_produtos_usuarios_fk FOREIGN KEY ( id_usuario_log )
        REFERENCES dealwish.usuarios ( id );

ALTER TABLE dealwish.grp_usr
    ADD CONSTRAINT grp_usr_usuarios_fk FOREIGN KEY ( id_usuario_log )
        REFERENCES dealwish.usuarios ( id );

ALTER TABLE dealwish.grp_usr
    ADD CONSTRAINT grupos_usr_grp_permissoes_fk FOREIGN KEY ( id_grp_permissao )
        REFERENCES dealwish.grp_permissoes ( id );

ALTER TABLE dealwish.grp_usr
    ADD CONSTRAINT grupos_usr_usuarios_fk FOREIGN KEY ( id_usuario )
        REFERENCES dealwish.usuarios ( id );

ALTER TABLE dealwish.ofertas
    ADD CONSTRAINT ofertas_desejos_fk FOREIGN KEY ( id_desejo )
        REFERENCES dealwish.desejos ( id );

ALTER TABLE dealwish.ofertas
    ADD CONSTRAINT ofertas_empresas_fk FOREIGN KEY ( id_empresa )
        REFERENCES dealwish.empresas ( id );

ALTER TABLE dealwish.ofertas
    ADD CONSTRAINT ofertas_faturas_fk FOREIGN KEY ( id_fatura )
        REFERENCES dealwish.faturas ( id );

ALTER TABLE dealwish.ofertas
    ADD CONSTRAINT ofertas_situacoes_fk FOREIGN KEY ( id_situacao )
        REFERENCES dealwish.situacoes ( id );

ALTER TABLE dealwish.ofertas
    ADD CONSTRAINT ofertas_usuarios_fk FOREIGN KEY ( id_usuario )
        REFERENCES dealwish.usuarios ( id );

ALTER TABLE dealwish.ofertas
    ADD CONSTRAINT ofertas_usuarios_fkv1 FOREIGN KEY ( id_usuario_log )
        REFERENCES dealwish.usuarios ( id );

ALTER TABLE dealwish.planos
    ADD CONSTRAINT planos_usuarios_fk FOREIGN KEY ( id_usuario_log )
        REFERENCES dealwish.usuarios ( id );

ALTER TABLE dealwish.tokens
    ADD CONSTRAINT tokens_usuarios_fk FOREIGN KEY ( id_usuario )
        REFERENCES dealwish.usuarios ( id );

ALTER TABLE dealwish.tp_produtos
    ADD CONSTRAINT tp_produtos_grp_produtos_fk FOREIGN KEY ( id_grp_prod )
        REFERENCES dealwish.grp_produtos ( id );

ALTER TABLE dealwish.tp_produtos
    ADD CONSTRAINT tp_produtos_situacoes_fk FOREIGN KEY ( id_situacao )
        REFERENCES dealwish.situacoes ( id );

ALTER TABLE dealwish.tp_produtos
    ADD CONSTRAINT tp_produtos_usuarios_fk FOREIGN KEY ( id_usuario_log )
        REFERENCES dealwish.usuarios ( id );

ALTER TABLE dealwish.usuarios
    ADD CONSTRAINT usuarios_cidades_fk FOREIGN KEY ( id_cidade_ap )
        REFERENCES dealwish.cidades ( id );

ALTER TABLE dealwish.usuarios
    ADD CONSTRAINT usuarios_empresas_fk FOREIGN KEY ( id_empresa )
        REFERENCES dealwish.empresas ( id );

ALTER TABLE dealwish.usuarios
    ADD CONSTRAINT usuarios_situacoes_fk FOREIGN KEY ( id_situacao )
        REFERENCES dealwish.situacoes ( id );

ALTER TABLE dealwish.usuarios
    ADD CONSTRAINT usuarios_usuarios_fk FOREIGN KEY ( id_usuario_log )
        REFERENCES dealwish.usuarios ( id );


  CREATE OR REPLACE function dealwish.proc_cidades_insert_update_trg()  RETURNS trigger AS $$
  BEGIN
    NEW.uf := upper(NEW.uf);
	RETURN NEW;	
  END;
  $$ LANGUAGE plpgsql;
  
  CREATE TRIGGER cidades_insert_update_trg
  AFTER INSERT OR 
  UPDATE ON dealwish.cidades for each row 
  execute procedure dealwish.proc_cidades_insert_update_trg(); 


INSERT INTO dealwish.situacoes (
    id,
    descricao,
    contratos,
    usuarios,
    desejos,
    faturas,
    ofertas,
    produtos
) VALUES (
    1,
    'Ativo',
    'S',
    'S',
    'S',
    'N',
    'S',
    'S'
);

INSERT INTO dealwish.situacoes (
    id,
    descricao,
    contratos,
    usuarios,
    desejos,
    faturas,
    ofertas,
    produtos
) VALUES (
    2,
    'Inativo',
    'N',
    'S',
    'N',
    'N',
    'S',
    'S'
);

INSERT INTO dealwish.situacoes (
    id,
    descricao,
    contratos,
    usuarios,
    desejos,
    faturas,
    ofertas,
    produtos
) VALUES (
    3,
    'Bloqueado',
    'S',
    'S',
    'N',
    'N',
    'N',
    'N'
);

INSERT INTO dealwish.situacoes (
    id,
    descricao,
    contratos,
    usuarios,
    desejos,
    faturas,
    ofertas,
    produtos
) VALUES (
    4,
    'Encerrado',
    'S',
    'N',
    'N',
    'N',
    'N',
    'N'
);

INSERT INTO dealwish.situacoes (
    id,
    descricao,
    contratos,
    usuarios,
    desejos,
    faturas,
    ofertas,
    produtos
) VALUES (
    6,
    'Realizado',
    'N',
    'N',
    'S',
    'N',
    'N',
    'N'
);

INSERT INTO dealwish.situacoes (
    id,
    descricao,
    contratos,
    usuarios,
    desejos,
    faturas,
    ofertas,
    produtos
) VALUES (
    7,
    'Não realizado',
    'N',
    'N',
    'S',
    'N',
    'N',
    'N'
);

INSERT INTO dealwish.situacoes (
    id,
    descricao,
    contratos,
    usuarios,
    desejos,
    faturas,
    ofertas,
    produtos
) VALUES (
    8,
    'Aberta',
    'N',
    'N',
    'N',
    'N',
    'N',
    'N'
);

INSERT INTO dealwish.situacoes (
    id,
    descricao,
    contratos,
    usuarios,
    desejos,
    faturas,
    ofertas,
    produtos
) VALUES (
    9,
    'A liquidar',
    'N',
    'N',
    'N',
    'S',
    'N',
    'N'
);

INSERT INTO dealwish.situacoes (
    id,
    descricao,
    contratos,
    usuarios,
    desejos,
    faturas,
    ofertas,
    produtos
) VALUES (
    10,
    'Liquidada',
    'N',
    'N',
    'N',
    'S',
    'N',
    'N'
);

INSERT INTO dealwish.situacoes (
    id,
    descricao,
    contratos,
    usuarios,
    desejos,
    faturas,
    ofertas,
    produtos
) VALUES (
    11,
    'Cancelada',
    'N',
    'N',
    'N',
    'S',
    'N',
    'N'
);

INSERT INTO dealwish.situacoes (
    id,
    descricao,
    contratos,
    usuarios,
    desejos,
    faturas,
    ofertas,
    produtos
) VALUES (
    12,
    'Gerar remessa nota fiscal',
    'N',
    'N',
    'N',
    'S',
    'N',
    'N'
);

    INSERT INTO dealwish.situacoes (
        id,
        descricao,
        contratos,
        usuarios,
        desejos,
        faturas,
        ofertas,
        produtos
    ) VALUES (
        13,
        'Gerar cobrança',
        'N',
        'N',
        'N',
        'S',
        'N',
        'N'
    );


-- senha: admin
insert into dealwish.usuarios (email, senha, nome, cpf, aplicativo, retaguarda, empresa, id_situacao) 
values ('admin@dealwish.xyz', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 'Admin', '20277722004','N','S', 'N', 1);

  CREATE OR REPLACE function dealwish.proc_permissoes_insert_update_trg()  RETURNS trigger AS $$
  BEGIN
     NEW.codigo := lower(NEW.codigo);
    IF NEW.descricao IS NULL THEN
        NEW.descricao :=NEW.codigo;
    END IF;
	RETURN NEW;
  END;
  $$ LANGUAGE plpgsql;
  
  CREATE TRIGGER permissoes_insert_update_trg
  before INSERT OR 
  UPDATE ON dealwish.permissoes for each row 
  execute procedure dealwish.proc_permissoes_insert_update_trg(); 


INSERT INTO dealwish.grp_permissoes (
    descricao,
    codigo
) VALUES (
    'Tecnologia da Informação',
    'tin'
);

INSERT INTO dealwish.grp_permissoes (
    descricao,
    codigo
) VALUES (
    'Aplicativo',
    'app'
);

INSERT INTO dealwish.grp_permissoes (
    descricao,
    codigo
) VALUES (
    'Backoffice Administrativo',
    'bka'
);

INSERT INTO dealwish.grp_permissoes (
    descricao,
    codigo
) VALUES (
    'Backoffice Comercial',
    'bkc'
);

INSERT INTO dealwish.grp_permissoes (
    descricao,
    codigo
) VALUES (
    'Backoffice Financeiro',
    'bkf'
);

INSERT INTO dealwish.grp_permissoes (
    descricao,
    codigo
) VALUES (
    'Backoffice Informações',
    'bki'
);

INSERT INTO dealwish.grp_permissoes (
    descricao,
    codigo
) VALUES (
    'Frontoffice Operação',
    'fto'
);

    INSERT INTO dealwish.grp_permissoes (
        descricao,
        codigo
    ) VALUES (
        'Frontoffice Administrativo',
        'fta'
    );


insert into dealwish.grp_usr (id_usuario, id_grp_permissao) values ((select id from dealwish.usuarios where email = 'admin@dealwish.xyz'), (select id from dealwish.grp_permissoes where codigo = 'tin'));


INSERT INTO dealwish.configuracoes (
    codigo,
    valor, id_usuario_log
) VALUES (
    'codigo_empresa',
    '111', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
);


INSERT INTO dealwish.configuracoes (
    codigo,
    valor, id_usuario_log
) VALUES (
    'nome_empresa',
    'Dealwish', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
);

INSERT INTO dealwish.configuracoes (
    codigo,
    valor, id_usuario_log
) VALUES (
    'numero_banco',
    '237', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
);

INSERT INTO dealwish.configuracoes (
    codigo,
    valor, id_usuario_log
) VALUES (
    'sequencia_remessa_boleto',
    '1', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
);

INSERT INTO dealwish.configuracoes (
    codigo,
    valor, id_usuario_log
) VALUES (
    'sequencia_dia_remessa_boleto',
    '1', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
);

INSERT INTO dealwish.configuracoes (
    codigo,
    valor, id_usuario_log
) VALUES (
    'ultimo_dia_geracao_remessa',
    '01/01/2019', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
);

INSERT INTO dealwish.configuracoes (
    codigo,
    valor, id_usuario_log
) VALUES (
    'carteira',
    '1', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
);

INSERT INTO dealwish.configuracoes (
    codigo,
    valor, id_usuario_log
) VALUES (
    'agencia',
    '2', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
);

INSERT INTO dealwish.configuracoes (
    codigo,
    valor, id_usuario_log
) VALUES (
    'conta_corrente',
    '3', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
);

INSERT INTO dealwish.configuracoes (
    codigo,
    valor, id_usuario_log
) VALUES (
    'inscricao_contribuinte',
    '1', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
);

INSERT INTO dealwish.configuracoes (
    codigo,
    valor, id_usuario_log
) VALUES (
    'serie_rps',
    '111', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
);

INSERT INTO dealwish.configuracoes (
    codigo,
    valor, id_usuario_log
) VALUES (
    'sequencia_remessa_nf',
    '1', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
);

INSERT INTO dealwish.configuracoes (
    codigo,
    valor, id_usuario_log
) VALUES (
    'cod_servico_prestado',
    '123456', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
);

INSERT INTO dealwish.configuracoes (
    codigo,
    valor, id_usuario_log
) VALUES (
    'discriminacao_servico',
    'Serviços de TI', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
);

INSERT INTO dealwish.configuracoes (
    codigo,
    valor, id_usuario_log
) VALUES (
    'num_max_tentativas_login',
    '5', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
);

INSERT INTO dealwish.configuracoes (
    codigo,
    valor, id_usuario_log
) VALUES (
    'ultima_atualizacao_produtos',
    '01/01/2019', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
);

    INSERT INTO dealwish.configuracoes (
        codigo,
        valor, id_usuario_log
    ) VALUES (
        'num_faturas_abertas_bloqueio_contrato',
        '3', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
    );

    INSERT INTO dealwish.configuracoes (
        codigo,
        valor, id_usuario_log
    ) VALUES (
        'texto_padrao_oferta_dealwish',
        'Encontramos esta oferta para você na loja ', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
    );
	
	
    INSERT INTO dealwish.configuracoes (
        codigo,
        valor, id_usuario_log
    ) VALUES (
        'pix_chave_conta',
        'pix@dealwish.com.br', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
    );
	
	
    INSERT INTO dealwish.configuracoes (
        codigo,
        valor, id_usuario_log
    ) VALUES (
        'pix_nome',
        'DEALWISH', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
    );
	
	
    INSERT INTO dealwish.configuracoes (
        codigo,
        valor, id_usuario_log
    ) VALUES (
        'pix_cidade',
        'BARUERI', (select id from dealwish.usuarios where email = 'admin@dealwish.xyz')
    );


INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_usuario' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/excluir_usuario' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/atualizar_usuario' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/reiniciar_senha' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/trocar_senha' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/incluir_grp_permissao_usuario' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/excluir_grp_permissao_usuario' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_permissoes_usuario' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_grp_permissoes_usuario' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_cidade' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_config' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/incluir_config' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/excluir_config' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/atualizar_config' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_grp_permissao' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/incluir_grp_permissao' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/excluir_grp_permissao' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/atualizar_grp_permissao' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/incluir_permissao_grupo' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/excluir_permissao_grupo' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_permissao_grupo' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_grp_produto' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_todos_grp_produto' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/incluir_grp_produto' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/excluir_grp_produto' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/atualizar_grp_produto' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_permissao' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_plano' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/incluir_plano' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/excluir_plano' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/atualizar_plano' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_situacao' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_tp_produto' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/incluir_tp_produto' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/atualizar_tp_produto' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/excluir_tp_produto' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_desejo' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/incluir_desejo' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/excluir_desejo' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/atualizar_desejo' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/atualizar_situacao_desejo' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_empresa' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/incluir_empresa' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/excluir_empresa' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/atualizar_empresa' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/incluir_usr_emp' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_usr_emp' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/excluir_usr_emp' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_contrato' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/incluir_contrato' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/excluir_contrato' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/atualizar_contrato' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_fatura' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/incluir_fatura' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/excluir_fatura' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/atualizar_fatura' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_oferta' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/incluir_oferta' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/excluir_oferta' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/atualizar_oferta' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_faturas_abertas' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/efetivar_faturas_abertas' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/incluir_oferta_lote' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/atualizar_situacao_oferta' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/atualizar_like_unlike_oferta' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/atualizar_lida_oferta' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/processar_retorno_boleto' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/processar_retorno_nf' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_ultima_atualizacao_produtos' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/bloquear_contratos_inadimplentes' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/desbloquear_contrato' );

INSERT INTO dealwish.permissoes ( codigo ) VALUES ( 'api/consultar_qualificacao' );

insert into dealwish.permissoes (codigo) values ('api/consultar_indicadores');

DELETE FROM dealwish.perm_grp;


    INSERT INTO dealwish.perm_grp (id_grp_permissao, id_permissao)
        SELECT
            (
                SELECT
                    id
                FROM
                    dealwish.grp_permissoes
                WHERE
                    codigo = 'tin'
            ),
            p.id
        FROM
            dealwish.permissoes p
        WHERE
            p.id NOT IN (
                SELECT
                    id_permissao
                FROM
                    dealwish.perm_grp
                WHERE
                    id_grp_permissao = (
                        SELECT
                            id
                        FROM
                            dealwish.grp_permissoes
                        WHERE
                            codigo = 'tin'
                    )
            );


    INSERT INTO dealwish.perm_grp (id_grp_permissao, id_permissao)
        SELECT

            (
                SELECT
                    id
                FROM
                    dealwish.grp_permissoes
                WHERE
                    codigo = 'bki'
            ),
            p.id
        FROM
            dealwish.permissoes p
        WHERE
            p.codigo LIKE '%consultar%'
            OR codigo = 'api/trocar_senha'
			OR codigo = 'api/consultar_indicadores'
			;


    INSERT INTO dealwish.perm_grp (id_grp_permissao, id_permissao)
        SELECT
            (
                SELECT
                    id
                FROM
                    dealwish.grp_permissoes
                WHERE
                    codigo = 'app'
            ),
            p.id
        FROM
            dealwish.permissoes p
        WHERE
            p.codigo IN (
                'api/atualizar_lida_oferta',
                'api/atualizar_like_unlike_oferta',
                'api/atualizar_situacao_desejo',
                'api/atualizar_usuario',
                'api/consultar_desejo',
                'api/consultar_oferta',
                'api/consultar_todos_grp_produto',
                'api/consultar_tp_produto',
                'api/incluir_desejo',
                'api/trocar_senha',
                'api/consultar_ultima_atualizacao_produtos',
				'api/consultar_empresa',
				'api/excluir_usuario'
            );


    INSERT INTO dealwish.perm_grp (id_grp_permissao, id_permissao)
        SELECT
      
            (
                SELECT
                    id
                FROM
                    dealwish.grp_permissoes
                WHERE
                    codigo = 'fta'
            ),
            p.id
        FROM
            dealwish.permissoes p
        WHERE
            p.codigo IN (
                'api/atualizar_usuario',
                'api/consultar_empresa',
                'api/consultar_fatura',
                'api/consultar_grp_permissao',
                'api/consultar_grp_permissoes_usuario',
                'api/consultar_grp_produto',
                'api/consultar_oferta',
                'api/consultar_situacao',
                'api/consultar_tp_produto',
                'api/consultar_usuario',
                'api/excluir_grp_permissao_usuario',
                'api/excluir_usuario',
                'api/incluir_grp_permissao_usuario',
                'api/incluir_usuario',
                'api/login_usuario',
                'api/reiniciar_senha',
                'api/trocar_senha',
                'api/consultar_cidade'
            );


    INSERT INTO dealwish.perm_grp  (id_grp_permissao, id_permissao)
        SELECT
     
            (
                SELECT
                    id
                FROM
                    dealwish.grp_permissoes
                WHERE
                    codigo = 'fto'
            ),
            p.id
        FROM
            dealwish.permissoes p
        WHERE
            p.codigo IN (
                'api/incluir_oferta_lote',
                'api/atualizar_situacao_oferta',
                'api/consultar_desejo',
                'api/consultar_empresa',
                'api/consultar_grp_produto',
                'api/consultar_oferta',
                'api/consultar_situacao',
                'api/consultar_tp_produto',
                'api/consultar_usuario',
                'api/incluir_oferta',
                'api/login_usuario',
                'api/reiniciar_senha',
                'api/retorno_oferta_lote',
                'api/trocar_senha',
                'api/consultar_cidade'
            );


    INSERT INTO dealwish.perm_grp  (id_grp_permissao, id_permissao)
        SELECT
          
            (
                SELECT
                    id
                FROM
                    dealwish.grp_permissoes
                WHERE
                    codigo = 'bka'
            ),
            p.id
        FROM
            dealwish.permissoes p
        WHERE
            p.codigo IN (
                'api/atualizar_config',
                'api/atualizar_grp_produto',
                'api/atualizar_plano',
                'api/atualizar_tp_produto',
                'api/atualizar_usuario',
                'api/consultar_cidade',
                'api/consultar_config',
                'api/consultar_desejo',
                'api/consultar_grp_permissao',
                'api/consultar_grp_permissoes_usuario',
                'api/consultar_grp_produto',
                'api/consultar_oferta',
                'api/consultar_plano',
                'api/consultar_situacao',
                'api/consultar_tp_produto',
                'api/consultar_usuario',
                'api/excluir_desejo',
                'api/excluir_grp_permissao_usuario',
                'api/excluir_grp_produto',
                'api/excluir_oferta',
                'api/excluir_plano',
                'api/excluir_tp_produto',
                'api/excluir_usuario',
                'api/incluir_grp_permissao_usuario',
                'api/incluir_grp_produto',
                'api/incluir_plano',
                'api/incluir_tp_produto',
                'api/incluir_usuario',
                'api/reiniciar_senha',
                'api/trocar_senha',
                'api/consultar_qualificacao',
				'api/consultar_indicadores'
            );


    INSERT INTO dealwish.perm_grp (id_grp_permissao, id_permissao)
        SELECT
           
            (
                SELECT
                    id
                FROM
                    dealwish.grp_permissoes
                WHERE
                    codigo = 'bkc'
            ),
            p.id
        FROM
            dealwish.permissoes p
        WHERE
            p.codigo IN (
                'api/atualizar_contrato',
                'api/atualizar_empresa',
                'api/consultar_cidade',
                'api/consultar_config',
                'api/consultar_contrato',
                'api/consultar_empresa',
                'api/consultar_fatura',
                'api/consultar_grp_permissoes_usuario',
                'api/consultar_grp_produto',
                'api/consultar_plano',
                'api/consultar_situacao',
                'api/consultar_tp_produto',
                'api/consultar_usuario',
                'api/excluir_contrato',
                'api/excluir_empresa',
                'api/incluir_contrato',
                'api/incluir_empresa',
                'api/trocar_senha',
                'api/consultar_qualificacao',
				'api/incluir_oferta'
            );

    INSERT INTO dealwish.perm_grp (id_grp_permissao, id_permissao)
        SELECT
           
            (
                SELECT
                    id
                FROM
                    dealwish.grp_permissoes
                WHERE
                    codigo = 'bkf'
            ),
            p.id
        FROM
            dealwish.permissoes p
where p.codigo IN 
( 'api/atualizar_fatura',
                    'api/atualizar_plano',
                    'api/consultar_cidade',
                    'api/consultar_config',
                    'api/consultar_contrato',
                    'api/consultar_empresa',
                    'api/consultar_fatura',
                    'api/consultar_faturas_abertas',
                    'api/consultar_grp_permissao',
                    'api/consultar_grp_permissoes_usuario',
                    'api/consultar_plano',
                    'api/consultar_situacao',
                    'api/consultar_usuario',
                    'api/efetivar_faturas_abertas',
                    'api/excluir_fatura',
                    'api/excluir_plano',
                    'api/incluir_plano',
                    'api/trocar_senha',
                    'api/processar_retorno_boleto',
                    'api/processar_retorno_nf',
                    'api/bloquear_contratos_inadimplentes',
                    'api/desbloquear_contrato',
                    'api/consultar_qualificacao');



CREATE OR REPLACE function dealwish.proc_planos_insert_update_trg()  RETURNS trigger AS $$
 BEGIN
    NEW.visualizacao := upper(NEW.visualizacao);
	RETURN NEW;
END;
  $$ LANGUAGE plpgsql;


  CREATE TRIGGER planos_insert_update_trg
  before INSERT OR 
  UPDATE ON dealwish.planos for each row 
  execute procedure dealwish.proc_planos_insert_update_trg(); 
 



INSERT INTO dealwish.qualificacoes (
    id,
    descricao
) VALUES (
    0,
    'Sem Qualificação'
);

INSERT INTO dealwish.qualificacoes (
    id,
    descricao
) VALUES (
    1,
    'Pequeno Porte'
);

INSERT INTO dealwish.qualificacoes (
    id,
    descricao
) VALUES (
    2,
    'Médio Porte'
);

    INSERT INTO dealwish.qualificacoes (
        id,
        descricao
    ) VALUES (
        3,
        'Grande Porte'
    );

insert into dealwish.planos (descricao, qtd_ofertas, valor_mensal, valor_oferta, visualizacao, id_usuario_log)
values ('Try',0,0,0,'P', 1);

insert into dealwish.planos (descricao, qtd_ofertas, valor_mensal, valor_oferta, visualizacao, id_usuario_log)
values ('Basic',120,100,1,'N', 1);

insert into dealwish.planos (descricao, qtd_ofertas, valor_mensal, valor_oferta, visualizacao, id_usuario_log)
values ('Standard',300,200,0.80,'P', 1);

insert into dealwish.planos (descricao, qtd_ofertas, valor_mensal, valor_oferta, visualizacao, id_usuario_log)
values ('Premium',600,300,0.60,'E', 1);

insert into dealwish.usuarios (id, email, senha, nome, cpf, aplicativo, retaguarda, empresa, id_situacao, data_nasc) 
values (0, 'dummy', 'null', 'dummy', '00000000000','N','N', 'N', 2, '1900-01-01');

INSERT INTO dealwish.termo_servico (
        corrente,
        texto
    ) VALUES (
        'S',
        'Termos e condições de uso

Estes Termos e Condições de Uso (ou, simplesmente, “Termo”) regulam o modo de funcionamento, os termos e as condições aplicáveis ao uso do aplicativo Dealwish (“Aplicativo”). O Aplicativo é operado por DEALWISH LTDA., inscrita no CNPJ/MF sob o n° 99.999.999/0001-99, com sede na Rua X, 99, 00.000-00, Cidade/SP, Brasil (“DEALWISH”).

Leia este Termo com atenção para compreendê-lo bem antes de utilizar o Aplicativo. Ao se cadastrar você manifesta sua concordância. Uma vez aceito, o Termo vale como qualquer contrato escrito e assinado por você. Caso não concorde com todas as disposições deste Termo, basta não confirmar seu cadastro. Você reconhece que, neste caso, não será possível acessar ou utilizar plenamente o Aplicativo.

1. O APLICATIVO

O Aplicativo oferece uma maneira conveniente para receber ofertas de um desejo de comprar, permitindo escolha do vendedor e realizar a compra através de link disponibilizado pelo mesmo. O link é inteiramente de responsabilidade do vendedor.

O Aplicativo não tem custo adicional aos usuários, funcionando apenas como um serviço para conectar os usuários com os vendedores disponíveis, que não possuem relação com o DEALWISH.

2. OBRIGAÇÕES

Sem prejuízo das demais obrigações previstas neste Termo, o DEALWISH compromete-se a:

Disponibilizar informações sobre os vendedores, conforme fornecido por eles;

Disponibilizar informações corretas e atualizadas aos vendedores, conforme fornecidas pelo usuário;

Prestar suporte online através de email em info@dealwish.com.br.

Sem prejuízo das demais obrigações previstas neste Termo, você compromete-se a:

Fornecer informações corretas, atuais e completas durante seu uso do Aplicativo;

Manter-se atualizado com o disposto neste Termo e na Política de Privacidade.

Fica desde logo esclarecido, declarando-se o usuário ciente de que o serviço oferecido pela DEALWISH se relaciona apenas à disponibilização de uma plataforma para conectar aos vendedores, toda a relação de compra, bem como pagamentos, entrega, suporte e demais assuntos relacionados à transação são de responsabilidade integral do vendedor, a quem deverão ser direcionados quaisquer reclamações.

A DEALWISH não se responsabiliza pela capacidade técnica do usuário de utilizar o Aplicativo. A compatibilidade do Aplicativo com o seu dispositivo é puramente de responsabilidade do usuário, assim como a conexão à internet. A DEALWISH reserva o direito de, por consequência da evolução e aprimoramento do Aplicativo, alterar os requisitos de funcionamento a seu exclusivo e razoável critério.

Para utilização do Aplicativo, será necessário que o usuário faça login utilizando alguns dados pessoais, incluindo nome e CPF. O usuário estará sujeito a este Termo e a nossa Política de Privacidade, disponível em www.dealwish.com.br

A criação do usuário e senha da conta de acesso é de sua inteira responsabilidade, e você entende que os dados de acesso ao Aplicativo são pessoais e não podem ser compartilhados com terceiros, sendo você inteira e exclusivamente responsável por todo e qualquer uso da sua conta.

PODEMOS SUSPENDER, ENCERRAR OU EXCLUIR SUA CONTA E BLOQUEAR SEU USO DO APLICATIVO SE RAZOAVELMENTE ENTENDERMOS QUE HÁ MOTIVO PARA TANTO, como, por exemplo, o desrespeito a este Termo. Caso isto aconteça, você será notificado a respeito da medida e sua motivação.

3. PRAZO E EXTINÇÃO

Este Termo é válido e produz plenos efeitos até sua extinção, observada a sobrevivência das disposições sobre propriedade intelectual, garantias e limitação de responsabilidades, conforme disposto neste Termo.

O usuário pode rescindir este Termo a qualquer momento solicitando o cancelamento de sua conta através de email em info@dealwish.com.br.

A DEALWISH pode rescindir este Termo a qualquer momento e independentemente de notificação prévia, sem que seja devido qualquer pagamento ou indenização ao usuário, nas seguintes hipóteses: (a) caso o usuário não cumpra qualquer de suas obrigações estipuladas na cláusula 2.2 deste Termo, viole direito de propriedade intelectual da DEALWISH conforme disposto na cláusula 5, ou leis e regulamentos aplicáveis; (b) se razoavelmente e de boa-fé acreditarmos que a rescisão é medida necessária para proteger a segurança ou a propriedade dos funcionários da DEALWISH, dos revendedores ou de terceiros, ou para prevenir fraudes ou falhas de segurança; (c) no caso de ações ou omissões imputáveis ao usuário que prejudiquem a reputação ou a imagem da DEALWISH; e (d) no caso de encerramento do funcionamento Aplicativo.

4. LICENÇA DE USO LIMITADA E PROPRIEDADE INTELECTUAL

O Aplicativo está disponível para os usuários apenas em conformidade com o Termo e com a legislação aplicável. Ao concordar com o Termo, você receberá uma licença de uso LIMITADA, pessoal, não exclusiva, intransferível e revogável para instalar o Aplicativo em seu smartphone.

Você reconhece todos os direitos de propriedade intelectual da DEALWISH, incluindo mas não limitado a marcas, direitos sobre software, direitos autorais e demais direitos relativos a criações (“Material Protegido”).

Você se compromete a não realizar qualquer ato que possa ser entendido como indicativo de que possui direito, a qualquer título, sobre a titularidade, total ou parcial, do Material Protegido, inclusive decompilação do Aplicativo e acesso ao código fonte, e se obriga a fazer uso do Material Protegido apenas da maneira e com as finalidades especificadas neste Termo.

Você entende que uso do Aplicativo fora dos termos deste Termo pode configurar crime, conforme disposto no Título III do Código Penal, no Título V da Lei 9279/96 – Lei de Propriedade Industrial, e no Capítulo V da Lei 9609/98 – Lei de Software.

Os dispositivos desta cláusula sobrevirão à rescisão, por qualquer motivo, do Termo, permanecendo em efeito e vinculando as partes.

5. RESPONSABILIDADE

VOCÊ RESPONDE INTEGRALMENTE POR QUALQUER PERDA OU DANO CAUSADO À DEALWISH OU A TERCEIRO RESULTANTE DE FATO, ATO OU OMISSÃO IMPUTÁVEL A VOCÊ OU POR QUALQUER VIOLAÇÃO POR VOCÊ DE QUALQUER DAS DISPOSIÇÕES DESTE TERMO.

A DEALWISH NÃO RESPONDE POR PERDA OU DANO CAUSADO A VOCÊ OU TERCEIRO RESULTANTE DE FATO A ELA NÃO IMPUTÁVEL OU RESULTANTE DE MAU USO POR VOCÊ OU POR TERCEIRO, OU DE FALHA TÉCNICA OU MAU FUNCIONAMENTO DO SMARTPHONE OU OUTRO DISPOSITIVO USADO POR VOCÊ.

A DEALWISH não responde por qualquer atraso ou falha resultante de causas fora do seu controle, incluindo qualquer falha em executar este Termo devido a circunstâncias imprevisíveis ou a causas além do controle da DEALWISH, tais como caso fortuito ou de força maior.

Você concorda em defender, indenizar e isentar a DEALWISH contra qualquer perda, reivindicação, obrigação, ação judicial ou outra despesa, incluindo honorários advocatícios, decorrentes do seu uso do Aplicativo ou de falsa, imprecisa ou inadequada representação feita por você como parte deste Termo ou de qualquer outra maneira.

O Aplicativo é fornecido a você da forma “como está”, sem garantias ou representações de qualquer tipo, expressas ou implícitas. A DEALWISH reserva o direito de alterar o Aplicativo, no todo ou em parte, a seu exclusivo e razoável critério. A DEALWISH se isenta de qualquer responsabilidade nos limites da lei aplicável, exceto conforme disposto neste Termo ou em outro acordo entre as partes.

Os dispositivos desta cláusula sobrevirão à rescisão, por qualquer motivo, do Termo, permanecendo em efeito e vinculando as partes.

6. DISPOSIÇÕES GERAIS

Podemos, a qualquer tempo, atualizar e de qualquer forma alterar este Termo, com pleno efeito a partir da publicação da versão atualizada do Termo no Aplicativo. Se as atualizações ou modificações a este Termo forem substanciais, você será notificado, e, neste caso, você deverá ler, compreender e manifestar concordância com a nova versão do Termo para continuar utilizando o Aplicativo. Tais alterações serão efetivas após a sua aceitação, e não terão efeitos retroativos. Se quaisquer alterações futuras ao Termo forem inaceitáveis para você ou fizerem com que não esteja mais de acordo ou em conformidade com este Termo, você deverá interromper qualquer uso do Aplicativo, sendo que poderemos suspender ou encerrar sua conta caso você não concorde ou não esteja em conformidade com a nova versão do Aplicativo.

Se qualquer cláusula ou item deste Termo for considerado ilegal, inválido ou inaplicável, no seu todo ou em parte, isso não afetará a legalidade, validade e eficácia das demais cláusulas do Termo, que continuarão plenamente válidas e eficazes.

Caso tenha dúvida a respeito deste Termo, entre em contato por e-mail em info@dealwish.com.br.

Esse Termo rege-se e deve ser interpretado de acordo com as leis do Brasil.

As partes elegem o Foro Central da Comarca de São Paulo – SP, com expressa renúncia a qualquer outro dentro dos limite do Código de Defesa do Consumidor, por mais privilegiado que seja, para dirimir quaisquer dúvidas ou controvérsias oriundas deste Termo.'
    );


CREATE OR REPLACE function proc_usuarios_insert_update_trg()  RETURNS trigger AS $$
BEGIN
    NEW.aplicativo := upper(NEW.aplicativo);
    NEW.retaguarda := upper(NEW.retaguarda);
    NEW.empresa := upper(NEW.empresa);
    NEW.email := lower(NEW.email);
    IF (NEW.aplicativo = 'S' AND NEW.id_cidade_ap IS NULL ) THEN
         RAISE EXCEPTION 'ID_CIDADE_AP obrigatório quando APLICATIVO = S';
    ELSIF (NEW.aplicativo = 'N' AND NEW.id_cidade_ap IS NOT NULL ) THEN
         RAISE EXCEPTION 'ID_CIDADE_AP permitido somente quando APLICATIVO = S';
    END IF;

    IF (NEW.empresa = 'S' AND NEW.id_empresa IS NULL ) THEN
         RAISE EXCEPTION 'ID_EMPRESA obrigatório quando EMPRESA = S';
    ELSIF (NEW.empresa = 'N' AND NEW.id_empresa IS NOT NULL ) THEN
         RAISE EXCEPTION 'ID_EMPRESA permitido somente quando EMPRESA = S';
    END IF;
	return new;
END;
 $$ LANGUAGE plpgsql;


  CREATE TRIGGER usuarios_insert_update_trg
  before INSERT OR 
  UPDATE ON dealwish.usuarios for each row 
  execute procedure proc_usuarios_insert_update_trg(); 