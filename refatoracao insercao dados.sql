IF @MERCADO = 'RF'
BEGIN
    INSERT INTO #TMP_FILAS_MENSAGERIA (DESCRICAO_FILA, NOM_FILR, TIPO, ORDEM)
    SELECT 'Renda Fixa Cadastrada', 'RF.NominalCadastro', 'RF', 1
    UNION ALL
    SELECT 'Renda Fixa Cancelada', 'RF.NominalCancelamento', 'RF', 2
    UNION ALL
    SELECT 'Renda Fixa Liquidada', 'RF.Liquidacao', 'RF', 3;
END
ELSE IF @MERCADO = 'DR'
BEGIN
    INSERT INTO #TMP_FILAS_MENSAGERIA (DESCRICAO_FILA, NOM_FILR, TIPO, ORDEM)
    SELECT 'Derivativo Cadastrado', 'DR.NDFcadastrado', 'DR', 1
    UNION ALL
    SELECT 'Derivativo Cancelado', 'DR.NDFcancelado', 'DR', 2
    UNION ALL
    SELECT 'Derivativo Liquidado', 'DR.NDFliquidado', 'DR', 3;
END
