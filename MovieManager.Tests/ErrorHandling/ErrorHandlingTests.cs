using FluentAssertions;
using Moq;
using MovieManager.Tests.Mocks;
using System;
using WebForms_MovieManager.Presenters;
using WebForms_MovieManager.Repositories;
using WebForms_MovieManager.Services;


namespace MovieManager.Tests.ErrorHandling
{
    public class ErrorHandlingTests
    {
        [Fact]
        public void Presenter_WhenExceptionOccurs_ShouldLogErrorAndShowUserMessag() 
        {
            //Arrange
            var mockView = new MockView();
            var mockRepository = new Mock<IMovieRepository>();
            var mockLogger = new Mock<IErrorLogger>();

            mockRepository
                .Setup(r => r.GetAllMovies())
                .Throws(new Exception("Database connection failed"));

            var presenter = new MoviePresenter(mockView, mockRepository.Object, mockLogger.Object);

            //Act
            mockView.RaiseLoadMovieEvent();

            //Assert
            mockLogger.Verify(l =>
                l.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.AtLeastOnce);
            
            mockView.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void ErrorLogger_WhenLoggingException_ShouldNotThrow()
        {
            //Arrange                        
            var logger = new ErrorLogger();
            var exception = new InvalidOperationException("Test exception");

            //Act
            Action act = () => logger.LogError(exception, "Additional test info");

            //Assert            
            act.Should().NotThrow();
        }

        [Fact]
        public void ErrorLogger_ShouldHandlenullHttpContex()
        {
            //Arrange                                    
            var logger = new ErrorLogger();
            var exception = new NullReferenceException("Test null exception");

            //Act
            Action act = () => logger.LogError(exception, "Additional test info");

            //Assert            
            act.Should().NotThrow();
        }
    }
}
