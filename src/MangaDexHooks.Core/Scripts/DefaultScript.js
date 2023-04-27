//Here you can write some basic javascript to interact with the embed
//The embeds will be rebuilt dynamically for each hook
//Because titles and descriptions provide multiple languages
//We'll fetch the English one or default to the first one if the English doesn't exist
const preferEn = (locale) => {
  if (!locale) return '';
  return locale['en'] || locale[Object.keys(locale)[0]];
};

const tags = manga.attributes.tags.map(t => preferEn(t.attributes.name)).join(', ');
//now we can modify the hook
builder
  .setContent('Hey guys! Checkout this new chapter that just dropped!')
  .setUser('Manga Notifications', 'https://static.index-0.com/image/kitsu.gif')
  .addEmbed(bob => {
    bob.setTitle(preferEn(manga.attributes.title))
      .setDescription(preferEn(manga.attributes.description))
      .setThumbnail(coverUrl)
      .setUrl('https://mangadex.org/chapter/' + chapter.id)
      .addField('Manga', '[mangadex](https://mangadex.org/manga/' + manga.id + ')', true)
      .addField('Rating', manga.attributes.contentRating || 'unknown', true)
      .addField('Demographic', manga.attributes.publicationDemographic || 'unknown', true)
      .addField('Tags', tags);
  });