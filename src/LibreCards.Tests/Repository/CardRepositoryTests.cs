﻿using LibreCards.Core.Entities;
using LibreCards.Core.Persistence;
using Moq;
using Xunit;

namespace LibreCards.Tests.Repository;

public class CardRepositoryTests
{
    private readonly Mock<IDataStorage> _dataStorageMock;
    private readonly ICardRepository _cardRepository;

    public CardRepositoryTests()
    {
        _dataStorageMock = new Mock<IDataStorage>();
        _cardRepository = new CardRepository(_dataStorageMock.Object);
    }

    [Fact]
    public void CardRepository_DefaultDraw_ShouldReturnOneCard()
    {
        ArrangeStorage(new[] { new Card { Id = 1, Text = "CardText" } });

        var cards = _cardRepository.DrawCards();

        Assert.Single(cards);
    }

    [Fact]
    public void CardRepository_DrawZero_ShouldReturnEmptyCollection()
    {
        ArrangeStorage(new[] { new Card { Id = 1, Text = "CardText" } });

        var cards = _cardRepository.DrawCards(0);

        Assert.Empty(cards);
    }

    [Fact]
    public void CardRepository_DrawManyCards_ShouldReturnCorrectNumberOfCards()
    {
        ArrangeStorageWithNumberOfCards(4);

        var cards = _cardRepository.DrawCards(2).ToList();
        cards.AddRange(_cardRepository.DrawCards(2));

        Assert.Equal(4, cards.Count);
    }

    [Fact]
    public void CardRepository_DrawTemplate_ShouldReturnValidTemplate()
    {
        var expected = new Template(Template.BlankPlaceholder);
        _dataStorageMock.Setup(ds => ds.DefaultTemplates).Returns(new[] { expected });

        var actual = _cardRepository.DrawTemplate();

        Assert.NotNull(actual);
        Assert.Equal(expected.Content, actual.Content);
        Assert.Equal(expected.BlankCount, actual.BlankCount);
    }

    private void ArrangeStorage(IEnumerable<Card> cards)
        => _dataStorageMock.Setup(ds => ds.DefaultCards).Returns(cards);

    private void ArrangeStorageWithNumberOfCards(int count)
        => _dataStorageMock.Setup(ds => ds.DefaultCards).Returns(Enumerable.Range(1, count).Select(i => new Card { Id = i, Text = "CardText" }));
}
